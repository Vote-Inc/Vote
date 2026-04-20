global using System.Threading;
global using System.Threading.Tasks;

global using Microsoft.AspNetCore.Mvc;

global using Vote.Domain.SeedWork;
global using Vote.Application.Commands.CastVote;
global using Vote.Application.Queries.GetVoteReceipt;
global using Vote.API.Extensions;
global using Vote.API.Requests;
global using Vote.Infrastructure;