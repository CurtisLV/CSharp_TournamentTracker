-- Get all the tournaments in DB that are active
CREATE PROCEDURE dbo.spTournaments_GetAll
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	from dbo.Tournaments
	where Active = 1;
END
GO