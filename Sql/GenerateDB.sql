CREATE TABLE [dbo].[Customers]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (MAX) NULL,
    [LastName] NVARCHAR (MAX) NULL,
    [Email] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
