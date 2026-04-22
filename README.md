# Vote API

ASP.NET Core 10 service for casting and verifying votes. Votes are stored in DynamoDB with voter IDs SHA-256 hashed to prevent duplicates without storing identity. Every cast vote appends to a hash-chained audit ledger.

## Project layout

```
Vote/
├── Vote.API/           # Controllers, Program.cs, Dockerfile
├── Vote.Application/   # Command/query handlers (CQRS)
├── Vote.Domain/        # Vote + AuditEntry entities, value objects
└── Vote.Infrastructure/ # DynamoDB repositories, unit of work
```

## Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `POST` | `/api/votes` | Bearer token (via nginx) | Cast a vote. Requires `X-Voter-Id` header injected by nginx. |
| `POST` | `/api/votes/verify` | None (public, rate-limited) | Verify a vote by receipt ID. |
| `GET` | `/health` | None | Health check. |

### Cast vote

Request:
```json
{ "electionId": "...", "candidateId": "..." }
```

Response `200`:
```json
{ "receiptId": "<uuid>" }
```

Errors:
- `401` — missing or invalid `X-Voter-Id`
- `409` — voter has already voted in this election
- `400` — invalid election or candidate ID

### Verify vote

Request:
```json
{ "receiptId": "<uuid>" }
```

Response `200`:
```json
{
  "electionId": "...",
  "candidateId": "...",
  "timestamp": "2025-01-01T12:00:00Z",
  "verified": true
}
```

Errors:
- `404` — receipt not found
- `429` — rate limit exceeded (10 requests/min per IP, burst 5)

> Verify is public — no token required. Rate limiting is enforced by nginx.

## Configuration

| Key | Description |
|-----|-------------|
| `DynamoDB__Region` | AWS region for votes table |
| `DynamoDB__TableName` | Votes table name (`evoting-votes`) |
| `DynamoDB__ServiceUrl` | Local DynamoDB URL — omit in production |
| `AuditDynamoDB__Region` | AWS region for audit table |
| `AuditDynamoDB__TableName` | Audit table name (`evoting-audit`) |
| `AuditDynamoDB__ServiceUrl` | Local audit DynamoDB URL — omit in production |
| `Frontend__Url` | Allowed CORS origin |

In production, `DynamoDB__ServiceUrl` and `AuditDynamoDB__ServiceUrl` must be set to `""` (empty string) so the SDK uses the region-based AWS endpoint instead of the localhost URL in `appsettings.Development.json`.

## Local development

```bash
cd Vote
docker compose up -d    # DynamoDB Local on :8000 (votes) and :8001 (audit)
dotnet run --project Vote.API
```

Tables must be created manually when using DynamoDB Local — they are not auto-created. Use the AWS CLI or NoSQL Workbench:

```bash
# votes table
aws dynamodb create-table \
  --table-name evoting-votes \
  --attribute-definitions AttributeName=electionId,AttributeType=S AttributeName=voterHash,AttributeType=S \
  --key-schema AttributeName=electionId,KeyType=HASH AttributeName=voterHash,KeyType=RANGE \
  --billing-mode PAY_PER_REQUEST \
  --endpoint-url http://localhost:8000

# audit table
aws dynamodb create-table \
  --table-name evoting-audit \
  --attribute-definitions AttributeName=electionId,AttributeType=S AttributeName=version,AttributeType=N AttributeName=receiptId,AttributeType=S \
  --key-schema AttributeName=electionId,KeyType=HASH AttributeName=version,KeyType=RANGE \
  --global-secondary-indexes '[{"IndexName":"receiptId-index","KeySchema":[{"AttributeName":"receiptId","KeyType":"HASH"}],"Projection":{"ProjectionType":"ALL"}}]' \
  --billing-mode PAY_PER_REQUEST \
  --endpoint-url http://localhost:8001
```

Swagger UI: `http://localhost:8083/swagger`

## DynamoDB tables

Both tables are created and owned by the vote Terraform stack (`infra/terraform/vote`).

**`evoting-votes`**

| Attribute | Type | Notes |
|-----------|------|-------|
| `electionId` | String (hash key) | Election identifier |
| `voterHash` | String (range key) | SHA-256 of voter ID — prevents duplicate votes |

**`evoting-audit`**

| Attribute | Type | Notes |
|-----------|------|-------|
| `electionId` | String (hash key) | Election identifier |
| `version` | Number (range key) | Monotonically increasing per election |
| `receiptId` | String | GSI hash key (`receiptId-index`) |
| `prevHash` | String | Hash of previous audit entry |
| `entryHash` | String | Hash of this entry's content |

## Docker

```bash
docker build -t vote .
docker run -p 8083:8083 \
  -e ASPNETCORE_URLS=http://+:8083 \
  -e DynamoDB__Region=us-east-1 \
  -e DynamoDB__TableName=evoting-votes \
  -e DynamoDB__ServiceUrl="" \
  -e AuditDynamoDB__Region=us-east-1 \
  -e AuditDynamoDB__TableName=evoting-audit \
  -e AuditDynamoDB__ServiceUrl="" \
  vote
```
