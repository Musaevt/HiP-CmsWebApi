using System;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Shared;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public partial class TopicsController
    {
        private AttachmentsManager _attachmentsManager;

        private void TopicsAttachmentsController()
        {
            _attachmentsManager = new AttachmentsManager(DbContext);
        }

        // GET api/topics/:id/attachments

        /// <summary>
        /// All attachments of the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">A list of attachments of the Topic {topicId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to get topic attachments</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Attachments")]
        [ProducesResponseType(typeof(IEnumerable<TopicAttachmentResult>), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAttachments([FromRoute]int topicId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            var attachments = _attachmentsManager.GetAttachments(topicId);
            if (attachments != null)
                return Ok(attachments);
            return NotFound();
        }

        // topicId not needed, but looks better at the api

        /// <summary>
        /// Specific attachment {attachmentId} of the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">the Id of the Topic attachment</param>                
        /// <response code="200">An attachment {attachmentId} of the topic {topicId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to get topic attachment</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Attachments/{attachmentId}")]
        [ProducesResponseType(typeof(StringWrapper), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAttachmet([FromRoute]int topicId, [FromRoute]int attachmentId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            try
            {
                var attachment = _attachmentsManager.GetAttachmentById(attachmentId);
                string fileName = Path.Combine(Constants.AttachmentFolder, topicId.ToString(), attachment.Path);
                var hash = DownloadManager.AddFile(fileName, HttpContext.Connection.RemoteIpAddress);
                return Ok(new StringWrapper() { Value = hash });
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create an attachment to the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="model">contains details about Topic attachment</param>                              
        /// <response code="200">Added attachment {attachmentId} successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to add topic attachment</response>        
        /// <response code="500">Internal server error</response>        
        /// <response code="401">User is denied</response>
        [HttpPost("{topicId}/Attachments")]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(EntityResult), 500)]
        public IActionResult PostAttachment([FromRoute]int topicId, [FromBody]AttachmentFormModel model)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _attachmentsManager.CreateAttachment(topicId, User.Identity.GetUserIdentity(), model);

            if (result.Success)
                return Ok(result);
            return InternalServerError(result);
        }


        // POST api/topics/:id/attachments

        /// <summary>
        /// Add an file to the attachment to the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">Id of the attachment</param>                
        /// <param name="file">The file to be attached with the topic</param>                
        /// <response code="200">Added attachment {attachmentId} successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to add topic attachment</response>        
        /// <response code="500">Internal server error</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/Attachments/{attachmentId}")]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(EntityResult), 500)]
        public IActionResult PutAttachment([FromRoute]int topicId, [FromRoute] int attachmentId, [FromForm]IFormFile file)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            if (file == null)
                ModelState.AddModelError("file", "File is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _attachmentsManager.PutAttachment(attachmentId, User.Identity.GetUserIdentity(), file);

            if (result.Success)
                return Ok(result);
            return InternalServerError(result);
        }

        // DELETE api/topics/:id/attachments

        /// <summary>
        /// Delete an attachment {attachmentId} in the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">The Id of the attachment</param>                
        /// <response code="200">Attachment {attachmentId} deleted successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to delete topic attachment</response>                
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}/Attachments/{attachmentId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteAttachment([FromRoute]int topicId, [FromRoute] int attachmentId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            if (_attachmentsManager.DeleteAttachment(topicId, attachmentId))
                return Ok();
            return NotFound();
        }
    }
}