using Gym_Boo.ControllerApi.Dtos;
using Gym_Boo.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Boo.ControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // POST /api/reviews/instructor
    // POST /api/reviews/session
    // POST /api/reviews/facility
    [HttpPost("{reviewType}")]
    public async Task<IActionResult> CreateReview(string reviewType, [FromBody] CreateReviewDto reviewDto)
    {
        try
        {
            var review = await _reviewService.CreateReviewAsync(reviewType, reviewDto);

            return Created();
            // 201 Created (To use later)
            //return CreatedAtAction(
            //    actionName: "GetById",
            //    routeValues: new { id = review.Id },
            //    value: review
            //);
        }
        catch (ArgumentException ex)
        {
            // 400 Bad Request if the reviewType in the URL does not match the enum
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // 400/403 Bad Request if the member did not attend the class
            return BadRequest(new { message = ex.Message });
        }
    }

}