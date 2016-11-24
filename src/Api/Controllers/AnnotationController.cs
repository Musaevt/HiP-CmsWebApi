﻿using System.Collections.Generic;
using Api.Data;
using Microsoft.Extensions.Logging;
using Api.Managers;
using Microsoft.AspNetCore.Mvc;
using Api.Models.AnnotationTag;
using Api.Permission;
using Api.Utility;
using Api.Models;
using System;

namespace Api.Controllers
{
    public class AnnotationController : ApiController
    {
        private AnnotationTagManager tagManager;
        private AnnotationPermissions annotationPermissions;

        public AnnotationController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            tagManager = new AnnotationTagManager(dbContext);
            annotationPermissions = new AnnotationPermissions(dbContext);
        }

        #region GET

        // Get api/annotation/Tags

        /// <summary>
        /// All Annotation Tags save in the system
        /// </summary>
        /// <param name="IncludeDeleted">Include already deleted, but still used Tags?</param>
        /// <response code="200">A List of AnnotationTagResults</response>
        /// <response code="204">There are no Tags in the system</response>
        [HttpGet("Tags")]
        [ProducesResponseType(typeof(IEnumerable<AnnotationTagResult>), 200)]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult GetAllTags(bool IncludeDeleted = false)
        {
            var tags = tagManager.getAllTags(IncludeDeleted);
            return Ok(tags);
        }

        // Get api/Annotation/Tags/:id

        /// <summary>
        /// A specific Tag save in the system
        /// </summary>
        /// <param name="id">The id of the Tag</param>
        /// <response code="200">A AnnotationTagResult</response>
        /// <response code="404">There is no Tag {id} in the system</response>
        [HttpGet("Tags/{id}")]
        [ProducesResponseType(typeof(AnnotationTagResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetTag(int id)
        {
            try
            {
                var tag = tagManager.getTag(id);
                return Ok(tag);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }


        // Get api/Annotation/Tags/:id/ChildTags

        /// <summary>
        /// All ChildTags from {id}
        /// </summary>
        /// <param name="id">The id of the parent tag</param>
        /// <response code="200">A List of AnnotationTagResults</response>
        /// <response code="204">The parent Tag {id} has no Children</response>
        [HttpGet("Tags/{id}/ChildTags")]
        [ProducesResponseType(typeof(List<AnnotationTagResult>), 200)]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult GetChildTagsOf(int id)
        {
            var tags = tagManager.getChildTagsOf(id);
            return Ok(tags);
        }

        #endregion

        #region POST

        // Post api/Annotation/Tags

        /// <summary>
        /// Add a new Tag.
        /// </summary>
        /// <param name="tag">The tag to be added</param>
        /// <response code="200">Tag created</response>
        /// <response code="403">User not allowed to created Tags</response>
        /// <response code="400">Request was missformed</response>
        [HttpPost("Tags")]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(AnnotationTagFormModel tag)
        {
            if (!annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserId()))
                return Forbid();

            if (ModelState.IsValid)
            {
                var result = tagManager.AddTag(tag);
                if (result.Success)
                    return Ok(result);
                return BadRequest(result);
            }
            return BadRequest();
        }

        // Post api/Annotation/Tags/:parentId/ChildTags/:childId

        /// <summary>
        /// Add Tag {childId} as Child to Tag {parentId} 
        /// </summary>
        /// <param name="parentId">Tag to add Child to</param>
        /// <param name="childId">Tag to be added as Child</param>
        /// <response code="200">child added</response>
        /// <response code="403">User not allowed to edit Tags</response>
        /// <response code="400">Request was missformed</response>
        [HttpPost("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(int parentId, int childId)
        {
            if (!annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            bool success = tagManager.AddChildTag(parentId, childId);
            if (success)
                return Ok();
            return BadRequest();
        }

        #endregion

        #region PUT

        // Put api/Annotation/Tags/:Id

        /// <summary>
        /// Edit Tag {Id} 
        /// </summary>
        /// <param name="Id">Tag to be edited</param>
        /// <param name="model">Date to be changed</param>
        /// <response code="200">Tag edited successful</response>
        /// <response code="403">User not allowed to edit Tags</response>
        /// <response code="404">No such Tag</response>
        [HttpPut("Tags/{Id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PutTag(int Id, AnnotationTagFormModel model)
        {
            if (!annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            var success = tagManager.EditTag(model, Id);
            if (success)
                return Ok();
            return NotFound();
        }

        #endregion

        #region DELETE

        // Delete api/Annotation/Tags/:Id

        /// <summary>
        /// Delete Tag {Id} 
        /// </summary>
        /// <param name="Id">Tag to be delete</param>
        /// <response code="200">Tag ddeleted successful</response>
        /// <response code="403">User not allowed to delete Tags</response>
        /// <response code="404">No such Tag</response>
        [HttpDelete("Tags/{Id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Delete(int Id)
        {
            if (!annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserId()))
                return Forbid();

            bool success = tagManager.DeleteTag(Id);
            if (success)
                return Ok();
            return NotFound();
        }

        /// <summary>
        /// remove Child {childId} from Tag {parentId} 
        /// </summary>
        /// <param name="parentId">Parent Tag to remove child from</param>
        /// <param name="childId">Child to be removed</param>
        /// <response code="200">Child removed successful</response>
        /// <response code="403">User not allowed to edit Tags</response>
        /// <response code="404">No such Tag</response>
        [HttpDelete("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteChildOf(int parentId, int childId)
        {
            if (!annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            bool success = tagManager.RemoveChildTag(parentId, childId);
            if (success)
                return Ok();
            return NotFound();
        }

        #endregion
    }
}