---Migration query for ProdDB

ALTER TABLE [dbo].[CollegeCourseSpecializationMapping]
    ADD [HigherSecondary] [bit] DEFAULT 0 NOT NULL,
	[Graduate] [bit] DEFAULT 0 NOT NULL


	ALTER TABLE [dbo].[CollegeStudentMapping]
    ADD [Feedback]       NVARCHAR (500)   NULL,
        [FeedbackRating] INT              NULL,
        [CourseId]       UNIQUEIDENTIFIER NULL;
ALTER TABLE [dbo].[Course]
    ADD [Type] NVARCHAR (50) NULL;


GO
ALTER TABLE [dbo].[Course]
    ADD [Type] NVARCHAR (50) NULL;

GO
PRINT N'Altering Table [dbo].[CourseMaster]...';


GO
ALTER TABLE [dbo].[CourseMaster]
    ADD [Type] NVARCHAR (50) NULL;


GO
PRINT N'Altering Table [dbo].[EducationMaster]...';


GO
ALTER TABLE [dbo].[EducationMaster]
    ADD [Type] NVARCHAR (50) NULL;


GO
PRINT N'Altering Table [dbo].[QuestionnaireResult]...';


GO
ALTER TABLE [dbo].[QuestionnaireResult]
    ADD [TrainingContentId] UNIQUEIDENTIFIER NULL;

	ALTER TABLE [dbo].Questionnaire
    ADD [TrainingContentId] UNIQUEIDENTIFIER NULL;

	CREATE TABLE [dbo].[TestScenario] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [CourseId]           UNIQUEIDENTIFIER NULL,
    [TrainingContentsId] UNIQUEIDENTIFIER NULL,
    [IsActive]           BIT              NULL,
    [NoOfQuestions]      INT              NULL,
    [CreatedBy]          NVARCHAR (450)   NULL,
    [UpdatedBy]          NVARCHAR (450)   NULL,
    [CreatedDate]        DATETIME         NULL,
    [UpdatedDate]        DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


update Questionnaire set IsActive = 'True' where IsActive = '1'