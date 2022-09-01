IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE UserName='Admin')
	INSERT [dbo].[User] ( [UserName],[Password], [FirstName], [LastName], [Email], [ContactNumber],[CreatedDate],[IsDelete]) 
	VALUES (N'Admin', N'aW5jaGNhcGVzZXB0MjABAx==', N'Admin', N'Admin',N'Admin@Cloud4code.com',N'2569865358',GetDate(), 0) 