@api
Feature: Spotkick API Response Schemas should be understood by third-party developers
As a third-party developer
I want to integrate with the Spotkick service
So that I can utilise its capabilities

    @user
    Scenario: I can retrieve a user
        Given I have access to the Spotkick API
        When I perform a 'GET' on the 'user/1' endpoint
        Then I get a response of '200'
        And the response body matches the json schema for a 'User'

    @artist
    Scenario: I can retrieve a user's followed artist
        Given I have access to the Spotkick API
        When I perform a 'GET' on the 'user/1/artist?location=Bristol%2C%20UK' endpoint
        Then I get a response of '200'
        And the response body matches the json schema for a 'List of Artists'

    @playlist
    Scenario: I can create a playlist for an artist
        Given I have access to the Spotkick API
        When I perform a 'POST' on the 'user/1/playlist' endpoint with the payload '{"artistIds": [1],"numberOfTracks": 1}'
        Then I get a response of '201'
        And the response body matches the json schema for a 'Playlist'