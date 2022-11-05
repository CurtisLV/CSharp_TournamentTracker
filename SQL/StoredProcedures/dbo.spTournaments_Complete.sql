CREATE PROCEDURE dbo.spTournaments_Complete
	@id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	update dbo.Tournaments
	set Active = 0
	where id = @id;
END
GO
