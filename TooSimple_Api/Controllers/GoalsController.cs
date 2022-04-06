using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Goals;
using TooSimple_Poco.Models.Budgeting;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoalsController : Controller
    {
        private readonly IGoalManager _goalManager;

        public GoalsController(IGoalManager goalManager)
        {
            _goalManager = goalManager;
        }

        /// <summary>
        /// Retrieves all goals for user.
        /// </summary>
        /// <param name="userId">
        /// ID of user to return goals for.
        /// </param>
        /// <returns>
        /// DTO containing IEnumerable of goals.
        /// </returns>
        [HttpGet("userId/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<GetGoalsDto> GetGoalsByUserId(string userId)
        {
            GetGoalsDto goalsDto = await _goalManager.GetGoalsByUserIdAsync(userId);
            goalsDto.Status = System.Net.HttpStatusCode.NotFound;
            return goalsDto;
        }

        /// <summary>
        /// Retrieves single goal and history by goal ID.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to return.
        /// </param>
        /// <returns>
        /// DTO containing goal and IEnumerable of goal contribution history.
        /// </returns>
        [HttpGet("goalId/{goalId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<GetGoalDto> GetGoalByGoalId(string goalId)
        {
            GetGoalDto goalDto = await _goalManager.GetGoalByGoalIdAsync(goalId);
            return goalDto;
        }

        /// <summary>
        /// Adds a new user created goal.
        /// </summary>
        /// <param name="goalDataModel">
        /// Model of new goal.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> Standard response with status and any error messages.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<BaseHttpResponse> AddNewGoal([FromBody]GoalDataModel goalDataModel)
        {
            BaseHttpResponse response = await _goalManager.AddNewGoalAsync(goalDataModel);
            return response;
        }

        /// <summary>
        /// Updates a goal with new data from user.
        /// </summary>
        /// <param name="goalDataModel">
        /// Goal model with new data.
        /// </param>
        /// <returns>
        /// Standard response with status and any error messages.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<BaseHttpResponse> UpdateGoal([FromBody] GoalDataModel goalDataModel)
        {
            BaseHttpResponse response = await _goalManager.UpdateGoalAsync(goalDataModel);
            return response;
        }

        /// <summary>
        /// Deletes goal by user request.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to delete.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> Standard response with status and any error messages.
        /// </returns>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<BaseHttpResponse> DeleteGoal(string goalId)
        {
            BaseHttpResponse response = await _goalManager.DeleteGoalAsync(goalId);
            return response;
        }
    }
}
