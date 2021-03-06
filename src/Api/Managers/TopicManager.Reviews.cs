﻿using System;
using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using Microsoft.EntityFrameworkCore;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public partial class TopicManager
    {

        public IEnumerable<TopicReviewResult> GetReviews(int topicId)
        {
            var result = new List<TopicReviewResult>();
            var reviews = DbContext.TopicReviews.Include(r => r.Reviewer).Where(rs => rs.TopicId == topicId).ToList();
            var reviewers = GetAssociatedUsersByRole(topicId, Role.Reviewer);

            foreach (var reviewer in reviewers)
            {
                try
                {
                    var review = reviews.Single(r => r.Reviewer.Email == reviewer.Identity);
                    result.Add(new TopicReviewResult(review));
                }
                catch (InvalidOperationException)
                {
                    result.Add(new TopicReviewResult(reviewer));
                }
            }
            return result;
        }

        public bool ChangeReviewStatus(string identity, int topicId, TopicReviewStatus status)
        {
            try
            {
                var user = GetUserByIdentity(identity);
                if (!DbContext.TopicReviews.Any(rs => rs.TopicId == topicId && rs.ReviewerId == user.Id))
                {
                    var review = new TopicReview()
                    {
                        TopicId = topicId,
                        ReviewerId = user.Id,
                        Status = status.Status
                    };
                    DbContext.TopicReviews.Add(review);
                }
                else
                {
                    var review =
                        DbContext.TopicReviews.Include(r => r.Reviewer)
                            .Single(rs => rs.TopicId == topicId && rs.ReviewerId == user.Id);
                    review.Status = status.Status;
                    DbContext.Update(review);
                }
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
