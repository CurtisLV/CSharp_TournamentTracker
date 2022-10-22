CREATE PROCEDURE dbo.spMatchupEntries_GetByMatchup
	@MatchupId int
AS
BEGIN
	SET NOCOUNT ON;

	select me.*
	from dbo.MatchupEntries me
	where me.MatchupId = @MatchupId

END
GO
