Feature: Authenticate with Spotify
	As a spotkick member
	I want to be able to authenticate using Spotify
	In order to create Spotify playlists

@e2e
Scenario: Navigating to the home page allows me to perform Spotify SSO
	Given I am on the 'Home' page
	When I click on the 'Fetch Followed Artists' button
	Then I am redirected to the 'Spotify SSO' page