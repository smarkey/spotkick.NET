Feature: Profile
	As an Acorn Answers Member
	I want to share select details about myself
	So that my presence is attractive, accurate & appropriate

@happy
Scenario: An Acorn Answers Member is issued a generated profile image on registration by default
	Given I am Signed in
	When I am on the 'Profile' page
	Then there is a profile image present

Scenario: An Acorn Answers Member has chosen to use a Gravatar profile image
	Given I am Signed in
	And I have chosen to use Gravatar
	When I am on the 'Profile' page
	Then the profile image is my Gravatar image

Scenario: Names - A-Z,a-z,unicode
