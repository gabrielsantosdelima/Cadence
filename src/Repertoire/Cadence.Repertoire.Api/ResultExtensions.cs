using Cadence.Repertoire.Domain;

namespace Cadence.Repertoire.Api
{
    public static class ResultExtensions
    {
        public static IResult ToProblem(this Result result, int statusCode = StatusCodes.Status400BadRequest)
        {
            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: statusCode);
        }
    }
}
