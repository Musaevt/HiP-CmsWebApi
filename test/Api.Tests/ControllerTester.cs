﻿using System.Security.Claims;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using MyTested.AspNetCore.Mvc;
using MyTested.AspNetCore.Mvc.Builders.Contracts.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests
{
    public class ControllerTester<T>
        where T: class
    {
        private readonly User Admin;
        private readonly User Student;
        private readonly User Supervisor;
        public AnnotationTag Tag1 { get; set; }
        public AnnotationTag Tag2 { get; set; }
        public AnnotationTag Tag3 { get; set; }
        public AnnotationTag Tag4 { get; set; }
        public AnnotationTagInstance TagInstance1 { get; set; }
        public AnnotationTagInstance TagInstance2 { get; set; }
        public AnnotationTagInstance TagInstance3 { get; set; }
        public AnnotationTagInstance TagInstance4 { get; set; }
        public AnnotationTagRelationRule RelationRule12 { get; set; }
        public AnnotationTagRelationRule RelationRule32 { get; set; }
        public AnnotationTagRelationRule RelationRule34 { get; set; }
        public AnnotationTagInstanceRelation Relation12 { get; set; }
        public AnnotationTagInstanceRelation Relation32 { get; set; }
        public AnnotationTagInstanceRelation Relation34 { get; set; }
        public Layer Layer2 { get; set; }
        public Layer Layer1 { get; set; }
        public LayerRelationRule LayerRelationRule { get; set; }

        public ControllerTester()
        {
            Admin = new User
            {
                Id = 1,
                Email = "admin@hipapp.de",
                Role = "Administrator"
            };
            Student = new User
            {
                Id = 2,
                Email = "student@hipapp.de",
                Role = "Student"
            };
            Supervisor = new User
            {
                Id = 3,
                Email = "supervisor@hipapp.de",
                Role = "Supervisor"
            };
            /*
             * Layer1   Layer2
             * |-tag1   |-tag2
             * |-tag3   |-tag4
             * 
             * Layer Relation Rules:
             * Layer1 -> Layer2
             * 
             * Annotation AnnotationTag Relations:
             * tag1 -> tag2
             * tag3 -> tag2
             * tag3 -> tag4
             */
            Layer1 = new Layer() { Id = 1, Name = "Time" };
            Layer2 = new Layer() { Id = 2, Name = "Perspective" };
            Tag1 = new AnnotationTag() { Id = 1, Layer = Layer1.Name };
            Tag2 = new AnnotationTag() { Id = 2, Layer = Layer2.Name };
            Tag3 = new AnnotationTag() { Id = 3, Layer = Layer1.Name };
            Tag4 = new AnnotationTag() { Id = 4, Layer = Layer2.Name };
            Tag1.ChildTags = new List<AnnotationTag>() { Tag3 };
            Tag2.ChildTags = new List<AnnotationTag>() { Tag4 };
            RelationRule12 = new AnnotationTagRelationRule() { Id = 3, SourceTagId = Tag1.Id, TargetTagId = Tag2.Id, Title = "Tag Relation Rule 1->2" };
            RelationRule32 = new AnnotationTagRelationRule() { Id = 5, SourceTagId = Tag3.Id, TargetTagId = Tag2.Id, Title = "Tag Relation Rule 3->2" };
            RelationRule34 = new AnnotationTagRelationRule() { Id = 7, SourceTagId = Tag3.Id, TargetTagId = Tag4.Id, Title = "Tag Relation Rule 3->4" };
            TagInstance1 = new AnnotationTagInstance(Tag1) { Id = 1 };
            TagInstance2 = new AnnotationTagInstance(Tag2) { Id = 2 };
            TagInstance3 = new AnnotationTagInstance(Tag3) { Id = 3 };
            TagInstance4 = new AnnotationTagInstance(Tag4) { Id = 4 };
            Relation12 = new AnnotationTagInstanceRelation(TagInstance1, TagInstance2) { Id = 3 };
            Relation32 = new AnnotationTagInstanceRelation(TagInstance3, TagInstance2) { Id = 5 };
            Relation34 = new AnnotationTagInstanceRelation(TagInstance3, TagInstance4) { Id = 7 };
            LayerRelationRule = new LayerRelationRule()
            {
                Id = 3,
                SourceLayer = Layer1,
                SourceLayerId = Layer1.Id,
                TargetLayer = Layer2,
                TargetLayerId = Layer2.Id,
                Color = "test-color",
                ArrowStyle = "test-style"
            };
        }

        /// <summary>
        /// Use this for bootstrapping your tests.
        /// Adds an admin, student and supervisor user to the database.
        /// </summary>
        /// <param name="userIdentity">The identity (i.e. the email address) of the user as whom you want to make the call. Defaults to admin.</param>
        /// <returns>An instance of IAndControllerBuilder, i.e. you can chain MyTested test method calls to the return value.</returns>
        public IAndControllerBuilder<T> TestController(string userIdentity = "admin@hipapp.de")
        {
            return MyMvc
                .Controller<T>()
                .WithAuthenticatedUser(user => user.WithClaim(ClaimTypes.Name, userIdentity))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(Admin, Student, Supervisor))
                );
        }

        public IAndControllerBuilder<T> TestControllerWithMockData(string userIdentity = "admin@hipapp.de")
        {
            return TestController(userIdentity)
                .WithDbContext(dbContext => dbContext                    
                    .WithSet<AnnotationTag>(db => db.AddRange(Tag1, Tag2, Tag3, Tag4))
                    .WithSet<Layer>(db => db.AddRange(Layer1, Layer2))
                    .WithSet<LayerRelationRule>(db => db.Add(LayerRelationRule))
                    .WithSet<AnnotationTagRelationRule>(db => db.AddRange(RelationRule12, RelationRule32, RelationRule34))
                    .WithSet<AnnotationTagInstance>(db => db.AddRange(TagInstance1, TagInstance2, TagInstance3, TagInstance4))                        
                );
        }
    }
}
