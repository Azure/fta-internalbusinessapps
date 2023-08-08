if not exists (select 1 from dbo.Expense)

BEGIN

INSERT INTO dbo.Expense (Purpose, ExpenseDate, Amount, EmployeeName, EmployeeEmail, ApproverName, ApproverEmail, Receipt)  values ('FastTrack team visit-1', '2017-7-10', 1500, 'Fakeuser One', 'fakeuser@contoso.com', 'Fakeuser One', 'fakeuser@contoso.com', 'http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg')
INSERT INTO dbo.Expense (Purpose, ExpenseDate, Amount, EmployeeName, EmployeeEmail, ApproverName, ApproverEmail, Receipt)  values ('FastTrack team visit-2', '2017-6-11', 990, 'Fakeuser Two', 'fakeuser2@contoso.com', 'Fakeuser Two', 'fakeuser2@contoso.com', 'http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg')
INSERT INTO dbo.Expense (Purpose, ExpenseDate, Amount, EmployeeName, EmployeeEmail, ApproverName, ApproverEmail, Receipt)  values ('FastTrack team visit-3', '2017-5-12', 1200, 'Fakeuser Three', 'fakeuser3@contoso.com', 'Fakeuser Three', 'fakeuser3@contoso.com', 'http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg')

END
