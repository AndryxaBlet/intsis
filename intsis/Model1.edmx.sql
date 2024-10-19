
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
    ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [FK_Answer_Rules];
GO
IF OBJECT_ID(N'[dbo].[FK_Rules_NameSis]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Rules] DROP CONSTRAINT [FK_Rules_NameSis];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Answer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Answer];
GO
IF OBJECT_ID(N'[dbo].[NameSis]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NameSis];
GO
IF OBJECT_ID(N'[dbo].[Rules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rules];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'NameSis'
CREATE TABLE [dbo].[NameSis] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Answer'
CREATE TABLE [dbo].[Answer] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [IDRule] int  NOT NULL,
    [Ans] nvarchar(250)  NOT NULL,
    [NextR] nvarchar(250)  NOT NULL,
    [Rec] nvarchar(250)  NULL
);
GO

-- Creating table 'Rules'
CREATE TABLE [dbo].[Rules] (
    [IDRule] int IDENTITY(1,1) NOT NULL,
    [IDSis] int  NOT NULL,
    [Text] nvarchar(250)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'NameSis'
ALTER TABLE [dbo].[NameSis]
ADD CONSTRAINT [PK_NameSis]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Answer'
ALTER TABLE [dbo].[Answer]
ADD CONSTRAINT [PK_Answer]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [IDRule] in table 'Rules'
ALTER TABLE [dbo].[Rules]
ADD CONSTRAINT [PK_Rules]
    PRIMARY KEY CLUSTERED ([IDRule] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [IDRule] in table 'Answer'
ALTER TABLE [dbo].[Answer]
ADD CONSTRAINT [FK_Answer_Rules]
    FOREIGN KEY ([IDRule])
    REFERENCES [dbo].[Rules]
        ([IDRule])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Answer_Rules'
CREATE INDEX [IX_FK_Answer_Rules]
ON [dbo].[Answer]
    ([IDRule]);
GO

-- Creating foreign key on [IDSis] in table 'Rules'
ALTER TABLE [dbo].[Rules]
ADD CONSTRAINT [FK_Rules_NameSis]
    FOREIGN KEY ([IDSis])
    REFERENCES [dbo].[NameSis]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rules_NameSis'
CREATE INDEX [IX_FK_Rules_NameSis]
ON [dbo].[Rules]
    ([IDSis]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------