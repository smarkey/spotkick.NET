Feature: Password Reset
	As an Acorn Answers Member who has forgotten my password
	I want to reset my password
	So that I regain access to Acorn Answers

@happy
Scenario: A member can request a password reset link
	Given I am on the 'Sign In' page
	When I click on the 'reset password' link
	And I am on the 'Password Reset' page
	When I enter my email address
	And click the submit button
	Then I receive a 'Password Reset' email

@happy
Scenario: A member can reset their password with the reset link provided by email
	Given I have clicked a password reset link
	When I enter my email address
	And I enter my new password
	And I re-enter my new password
	Then I arrive on the 'Home' page as a logged in user

Scenario: Validation of email & password