Feature: Parking Space Management
  As a parking operator
  I want to manage parking spaces and vehicle entries
  So that I can track vehicle parking and revenue

  Scenario: Create a new parking space
    Given branch with ID "1" exists
    When I create a parking space with number "A01" and type "1"
    Then the parking space should be created successfully
    And the status should be "Available"

  Scenario: Cannot create parking space with duplicate number in same branch
    Given a parking space exists with number "A01" in branch "1"
    When I try to create another space with number "A01" in the same branch
    Then the creation should fail with error "ParkingSpace.DuplicateNumber"

  Scenario: Register vehicle entry
    Given a parking space with ID "1" is available
    When I register a vehicle entry with license plate "ABC-1234" and model "Honda Civic"
    Then the vehicle entry should be registered successfully
    And the parking space status should change to "Occupied"

  Scenario: Register vehicle exit
    Given a vehicle is parked in space "1" for 60 minutes
    When I register a vehicle exit with amount 25.50
    Then the vehicle exit should be recorded successfully
    And the parking space should become "Available" again

  Scenario: Get parking space occupancy
    Given branch "1" has 10 parking spaces with 3 occupied
    When I get the occupancy for branch "1"
    Then the total spaces should be 10
    And the occupied spaces should be 3
    And the occupancy rate should be 30%
