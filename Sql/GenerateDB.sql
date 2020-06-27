DROP TABLE [dbo].[Tasks]
CREATE TABLE [dbo].[Tasks]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Status] NVARCHAR (MAX) NULL,
    [DueDate] [datetime2](7) NOT NULL,
    [DateCreated] [datetime2](7) NOT NULL,
    [DateModified] [datetime2](7) NULL,
    CONSTRAINT [PK_dbo.Tasks] PRIMARY KEY CLUSTERED ([Id] ASC)
);
