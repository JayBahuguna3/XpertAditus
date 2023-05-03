select * from UserProfile where MobileNumber = '8962270051'
select * from AspNetUsers where PhoneNumber = '8962270051'

declare @userprofileid nvarchar(max)
set @userprofileid = 'FFBC61DB-A5D5-477B-8207-45D4918F459C'

declare @usercourseid nvarchar(max)
set  @usercourseid  = (select UserCoursesID from UserCourses where UserProfileID = @userprofileid)

--select *  from CandidateTrainingStatus where UserCoursesID = @usercourseid
--select *  from DisclaimerAccetped where UserProfileID = @userprofileid
--select *  from QuestionnaireResult where CandidateResultId in (select CandidateResultId from CandidateResult where UserCoursesID = @usercourseid)
--select *  from CandidateResult where UserCoursesID = @usercourseid
--select *  from PaymentHistory where UserCoursesID = @usercourseid
--select *  from UserCourses where UserProfileID = @userprofileid

--delete from CandidateTrainingStatus where UserCoursesID = @usercourseid
--delete from DisclaimerAccetped where UserProfileID = @userprofileid
--delete from QuestionnaireResult where CandidateResultId in (select CandidateResultId from CandidateResult where UserCoursesID = @usercourseid)
--delete from CandidateResult where UserCoursesID = @usercourseid
--delete from PaymentHistory where UserCoursesID = @usercourseid
--delete from UserCourses where UserProfileID = @userprofileid


--user info--
select * from Address where UserProfileId = @userprofileid
select * from Qualification where UserProfileId = @userprofileid
select * from AppliedJobs where JobId in (select jobid from JobMaster where UserProfileId = @userprofileid)
select * from ApprovalJobs where JobId in (select jobid from JobMaster where UserProfileId = @userprofileid)
select * from JobMaster where UserProfileId = @userprofileid
select * from ShortlistedCandidates where CandidateID = @userprofileid
select * from ShortlistedCandidates where EmployerID = @userprofileid
select * from AppliedJobs where UserProfileId = @userprofileid
select * from UserProfile where UserProfileId = @userprofileid

--User delete --
delete from Address where UserProfileId = @userprofileid
delete from Qualification where UserProfileId = @userprofileid
delete from ShortlistedCandidates where JobId in (select jobid from JobMaster where UserProfileId = @userprofileid)
delete from AppliedJobs where JobId in (select jobid from JobMaster where UserProfileId = @userprofileid)
delete from ApprovalJobs where JobId in (select jobid from JobMaster where UserProfileId = @userprofileid)
delete from JobMaster where UserProfileId = @userprofileid
delete from AppliedJobs where UserProfileId = @userprofileid
delete from UserProfile where UserProfileId = @userprofileid
delete from AspNetUsers where PhoneNumber = '8962270051'