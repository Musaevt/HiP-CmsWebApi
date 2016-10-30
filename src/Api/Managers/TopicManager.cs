using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Api.Models.Entity;

namespace Api.Managers
{
    public class TopicManager : BaseManager
    {
        public TopicManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IQueryable<Topic> GetAllTopics(string query, string status, DateTime? deadline, bool onlyParents)
        {
            IQueryable<Topic> topics = from t in dbContext.Topics select t;
            if (!string.IsNullOrEmpty(query))
                topics = topics.Where(t => t.Title.Contains(query) || t.Description.Contains(query));

            if (!string.IsNullOrEmpty(status))
                topics = topics.Where(t => t.Status.CompareTo(status) == 0);

            if (deadline != null)
                topics = topics.Where(t => DateTime.Compare(t.Deadline, (DateTime)deadline) == 0);

            if (onlyParents)
                topics = topics.Except(from at in dbContext.AssociatedTopics
                                       join t in topics
                                       on at.ChildTopicId equals t.Id
                                       select t);
            return topics;
        }

        public virtual IQueryable<Topic> GetTopicsForUser(int userId)
        {
            return from tu in dbContext.TopicUsers
                   join t in dbContext.Topics
                   on tu.TopicId equals t.Id
                   where tu.UserId == userId
                   select t;
        }

        public virtual int GetTopicsCount()
        {
            return dbContext.Topics.Count();
        }

        public virtual Topic GetTopicById(int topicId)
        {
            return dbContext.Topics.Include(t => t.CreatedBy).FirstOrDefault(t => t.Id == topicId);
        }

        public virtual IEnumerable<User> GetAssociatedUsersByRole(int topicId, string role)
        {
            return dbContext.TopicUsers.Where(tu => (tu.Role == role && tu.TopicId == topicId)).Select(u => u.User).ToList();
        }

        public virtual IEnumerable<Topic> GetSubTopics(int topicId)
        {
            return dbContext.AssociatedTopics.Where(at => at.ParentTopicId == topicId).Select(at => at.ChildTopic).ToList();
        }

        public virtual IEnumerable<Topic> GetParentTopics(int topicId)
        {
            return dbContext.AssociatedTopics.Where(at => at.ChildTopicId == topicId).Select(at => at.ChildTopic).ToList();
        }

        public virtual AddEntityResult AddTopic(int userId, TopicFormModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var topic = new Topic(model);
                    topic.CreatedById = userId;

                    // Add User associations
                    AssociateUsersToTopicByRole(Role.Student, model.Students, topic);
                    AssociateUsersToTopicByRole(Role.Supervisor, model.Supervisors, topic);
                    AssociateUsersToTopicByRole(Role.Reviewer, model.Reviewers, topic);

                    // Add Topic Associations
                    topic.AssociatedTopics = AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

                    dbContext.Topics.Add(topic);

                    dbContext.SaveChanges();

                    transaction.Commit();
                    return new AddEntityResult() { Success = true, Value = topic.Id };
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new AddEntityResult() { Success = false, ErrorMessage = e.Message };
                }
            }
        }

        public virtual bool UpdateTopic(int userId, int topicId, TopicFormModel model)
        {
            var topic = dbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
            if (topic == null)
                return false;

            try
            {
                // TODO  topic.UpdatedById = userId;
                topic.Title = model.Title;
                topic.Status = model.Status;
                topic.Deadline = (DateTime)model.Deadline;
                topic.Description = model.Description;
                topic.Requirements = model.Requirements;


                // Add User associations
                AssociateUsersToTopicByRole(Role.Student, model.Students, topic);
                AssociateUsersToTopicByRole(Role.Supervisor, model.Supervisors, topic);
                AssociateUsersToTopicByRole(Role.Reviewer, model.Reviewers, topic);

                // Add Topic associations
                topic.AssociatedTopics = AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public bool ChangeTopicStatus(int userId, int topicId, string status)
        {
            var topic = dbContext.Topics.FirstOrDefault(t => t.Id == topicId);
            if (topic != null)
            {
                topic.Status = status;
                dbContext.Update(topic);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual bool DeleteTopic(int topicId)
        {
            var topic = dbContext.Topics.FirstOrDefault(u => u.Id == topicId);
            if (topic != null)
            {
                dbContext.Remove(topic);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }


        // Private Region

        private void AssociateUsersToTopicByRole(string role, int[] userIds, Topic topic)
        {
            var existingUsers = topic.TopicUsers.Where(tu => tu.Role == role).ToList();

            var newUsers = new List<TopicUser>();
            var removedUsers = new List<TopicUser>();

            if (userIds != null)
            {
                // new user?
                foreach (int userId in userIds)
                {
                    if (!existingUsers.Any(tu => (tu.UserId == userId && tu.Role == role)))
                        newUsers.Add(new TopicUser() { UserId = userId, Role = role });
                }
                // removed user?
                foreach (TopicUser existingUser in existingUsers)
                {
                    if (!userIds.Contains(existingUser.UserId))
                        removedUsers.Add(existingUser);
                }
            }

            topic.TopicUsers.AddRange(newUsers);
            topic.TopicUsers.RemoveAll(tu => removedUsers.Contains(tu));
        }

        private List<AssociatedTopic> AssociateTopicsToTopic(int topicId, int[] associatedTopicIds)
        {
            var associatedTopics = new List<AssociatedTopic>();

            if (associatedTopicIds != null)
            {
                foreach (int associatedTopicId in associatedTopicIds)
                {
                    associatedTopics.Add(new AssociatedTopic() { ParentTopicId = associatedTopicId });
                }
            }
            return associatedTopics;
        }
    }
}