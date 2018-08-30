Feature: Forum Registration
	As an Acorn Answers Visitior
	I want to register
	So that I can interact with other users

@happy @e2e
Scenario: An Acorn Answers Visitor can register with an email address and password
	Given I am on the 'Registration' page
	When I provide a 'nickname' that is 'valid'
	And I provide an 'email address' that is 'valid'
	And I provide a 'password' that is 'valid'
	And I provide a 'repeat password' that is 'valid'
	And I 'accept' the 'terms and conditions'
	When I click the 'Register' button
	Then I arrive on the 'Home' page as a logged in user
	And I receive a 'Welcome' email

@happy @e2e
Scenario Outline: An Acorn Answers Visitor can register with SSO
	Given I am on the 'Registration' page
	When I click the '<Social Platform>' link
	And I am diverted to the '<Social Platform>' service
	And I sign in to the '<Social Platform>' service
	Then I arrive on the 'Home' page as a logged in user
	And I receive a 'Welcome' email
	Examples: 
	| Social Platform |
	| Facebook        |
	| Twitter         |
	| LinkedIn        |
	| Google          |

@component
Scenario Outline: An Acorn Answers Member cannot register using an invalid nickname
	Given I am on the 'Registration' page
	When I provide a nickname that contains '<Description>'
	Then a validation message that reads '<Message>' is displayed
	And the 'Register' button is 'disabled'
	Examples:
	| Description        | Message                                                                                     |
	| special characters | The nickname needs to consist of A-Z,a-z,0-9 *some* special chars, max 16, no foul language |
	| over 16 characters | The nickname needs to consist of A-Z,a-z,0-9 *some* special chars, max 16, no foul language |
	| foul language      | The nickname needs to consist of A-Z,a-z,0-9 *some* special chars, max 16, no foul language |
	| blank              | The nickname is a mandatory field                                                           |

@component
Scenario Outline: An Acorn Answers Visitor cannot register using an invalid email address
	Given I am on the 'Registration' page
	When I provide an email address that is '<Description>'
	Then a validation message that reads '<Message>' is displayed
	And the 'Register' button is 'disabled'
	Examples: 
	| Description                         | Message                                                                                                  |
	| username missing                    | This email address must be a real email address                                                          |
	| username too short                  | This email address must be a real email address                                                          |
	| username greater than 64 characters | This email address must be a real email address                                                          |
	| username contains space             | This email address must be a real email address                                                          |
	| missing @                           | This email address must be a real email address                                                          |
	| mail server missing                 | This email address must be a real email address                                                          |
	| mail server too short               | This email address must be a real email address                                                          |
	| mail server contains space          | This email address must be a real email address                                                          |
	| missing .                           | This email address must be a real email address                                                          |
	| top level domain missing            | This email address must be a real email address                                                          |
	| top level domain too short          | This email address must be a real email address                                                          |
	| top level domain contains space     | This email address must be a real email address                                                          |
	| already in use                      | This email address is already in use. If you have lost your password, please use Forgotton Password link |
	| blank                               | The email address is a mandatory field                                                                   |

@component
Scenario Outline: An Acorn Answers Visitor cannot register using an invalid password
	Given I am on the 'Registration' page
	When I provide a 'password' that is '<Description>'
	When I provide a 'repeat password' that is '<Description>'
	Then a validation message that reads '<Message>' is displayed
	And the 'Register' button is 'disabled'
	Examples: 
	| Description        | Message                                                               |
	| over 10 characters | The password is invalid. It should be a minimum of 10 characters long |
	| does not match     | The password must match                                               |
	| blank              | The password is a mandatory field                                     |

@component
Scenario: An Acorn Answers Visitor cannot register without accepting the terms and conditions
	Given I am on the 'Registration' page
	And I provide an email address that is 'valid'
	And I provide a password that is 'valid'
	When I 'have not accepted' the terms and conditions
	Then the 'Register' button is 'disabled'