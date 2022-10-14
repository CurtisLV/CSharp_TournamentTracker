USE [Tournaments]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spTeams_GetAll]

AS
BEGIN

	SET NOCOUNT ON;

	select * 
	from dbo.Teams;
END
GO