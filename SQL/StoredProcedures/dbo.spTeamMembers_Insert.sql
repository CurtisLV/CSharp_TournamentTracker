
CREATE PROCEDURE dbo.spTeamMembers_Insert
	@TeamId int,
	@PersonId int,
	@id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	insert into dbo.TeamMembers(TeamId, PersonId)
	values (@TeamId, @PersonId)

	select @id = SCOPE_IDENTITY();
END
GO
