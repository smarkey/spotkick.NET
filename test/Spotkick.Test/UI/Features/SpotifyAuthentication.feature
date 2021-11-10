@ui
Feature: Authenticate with Spotify
As a spotkick member
I want to be able to authenticate using Spotify
In order to create Spotify playlists

    Scenario: Navigating to the home page allows me to perform Spotify SSO
        Given I am on the 'Home' page
        When I click on the 'Login to Spotify' button
        Then I am redirected to the 'Spotify SSO' page