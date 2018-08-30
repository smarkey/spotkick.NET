Feature: Forum Sign In
	As an Acorn Answers Member
	I want to sign in
	So that I can interact with other users

@happy
Scenario: An Acorn Answers Member can sign in with a valid and matching email address and password
	Given I am on the 'Sign In' page
	When I provide an email address that is 'valid'
	And I provide a password that is 'valid'
	And I click the 'Sign In' button
	Then I arrive on the 'Home' page as a logged in user

@happy
Scenario Outline: An Acorn Answers Visitor can sign in using SSO
	Given I am on the 'Sign In' page
	When I click the '<Social Service>' link
	Then I arrive on the 'Home' page as a logged in user
	Examples:
	| Social Service |
	| Facebook       |
	| Twitter        |
	| LinkedIn       |
	| Google         |

Scenario: An Acorn Answers Member cannot attempt to sign in more than 3 times
	Given I am on the 'Sign In' page
	When I provide an email address that is 'valid'
	And I provide a password that is 'invalid'
	And I click the 'Sign In' button 4 times
	Then a validation message that reads 'You have been locked out. You have failed to provide the correct password for this account 3 times' is displayed

Scenario: An Acorn Answers Visitor should be redirected to origin of sign in request
	Given I am on a 'Question' page
	And I am signed out
	When I attempt to answer the Question
	And I am redirected to the 'Sign In' page
	And I sign in
	Then I am on a 'Question' page
	And I am able to answer the Question