
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/19/2024 14:31:23
-- Generated from EDMX file: C:\Users\AndryxaBlet\Desktop\intsis\intsis\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [intsisIR311];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Answer_Rules]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LinearSystem_Answer] DROP CONSTRAINT [FK_Answer_Rules];
GO
IF OBJECT_ID(N'[dbo].[FK_Rules_NameSis]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LinearSystem_Question] DROP CONSTRAINT [FK_Rules_NameSis];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[LinearSystem_Answer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LinearSystem_Answer];
GO
IF OBJECT_ID(N'[dbo].[ExpSystem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpSystem];
GO
IF OBJECT_ID(N'[dbo].[LinearSystem_Question]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LinearSystem_Question];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ExpSystem'
CREATE TABLE [dbo].[ExpSystem] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'LinearSystem_Answer'
CREATE TABLE [dbo].[LinearSystem_Answer] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Id] int  NOT NULL,
    [Ans] nvarchar(250)  NOT NULL,
    [NextR] nvarchar(250)  NOT NULL,
    [Rec] nvarchar(250)  NULL
);
GO

-- Creating table 'LinearSystem_Question'
CREATE TABLE [dbo].[LinearSystem_Question] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IDSis] int  NOT NULL,
    [Text] nvarchar(250)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'ExpSystem'
ALTER TABLE [dbo].[ExpSystem]
ADD CONSTRAINT [PK_NameSis]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'LinearSystem_Answer'
ALTER TABLE [dbo].[LinearSystem_Answer]
ADD CONSTRAINT [PK_Answer]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'LinearSystem_Question'
ALTER TABLE [dbo].[LinearSystem_Question]
ADD CONSTRAINT [PK_Rules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Id] in table 'LinearSystem_Answer'
ALTER TABLE [dbo].[LinearSystem_Answer]
ADD CONSTRAINT [FK_Answer_Rules]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[LinearSystem_Question]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Answer_Rules'
CREATE INDEX [IX_FK_Answer_Rules]
ON [dbo].[LinearSystem_Answer]
    ([Id]);
GO

-- Creating foreign key on [IDSis] in table 'LinearSystem_Question'
ALTER TABLE [dbo].[LinearSystem_Question]
ADD CONSTRAINT [FK_Rules_NameSis]
    FOREIGN KEY ([IDSis])
    REFERENCES [dbo].[ExpSystem]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rules_NameSis'
CREATE INDEX [IX_FK_Rules_NameSis]
ON [dbo].[LinearSystem_Question]
    ([IDSis]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------