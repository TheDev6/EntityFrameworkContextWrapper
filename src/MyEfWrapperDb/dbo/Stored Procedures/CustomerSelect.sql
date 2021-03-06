﻿
CREATE PROCEDURE CustomerSelect @CustomerGuid UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	SELECT
		c.CustomerGuid
		,c.Name
	FROM Customer c
	WHERE c.CustomerGuid = @CustomerGuid
END