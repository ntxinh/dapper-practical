DROP TABLE [dbo].[Customers]
CREATE TABLE [dbo].[Customers]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (MAX) NULL,
    [LastName] NVARCHAR (MAX) NULL,
    [Email] NVARCHAR (MAX) NULL,

    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    [UpdatedBy] INT NOT NULL,
    [IsDeleted] BIT NOT NULL,

    CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

DROP TABLE [dbo].[Todos]
CREATE TABLE [dbo].[Todos]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Status] NVARCHAR (MAX) NULL,
    [DueDate] [datetime2](7) NOT NULL,
    [DateCreated] [datetime2](7) NOT NULL,

    [DateModified] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    [UpdatedBy] INT NOT NULL,
    [IsDeleted] BIT NOT NULL,

    CONSTRAINT [PK_dbo.Todos] PRIMARY KEY CLUSTERED ([Id] ASC)
);
