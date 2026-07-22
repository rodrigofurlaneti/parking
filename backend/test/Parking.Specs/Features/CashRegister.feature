Feature: Cash Register Management
  As a branch manager
  I want to manage cash registers
  So that I can track daily cash movements and balance

  Scenario: Open a cash register
    Given branch with ID "1" exists
    When I open a cash register with opening balance 1000.00
    Then the cash register should be opened successfully
    And the status should be "Open"

  Scenario: Cannot open another cash register when one is already open
    Given a cash register is already open for branch "1"
    When I try to open another cash register for the same branch
    Then the operation should fail with error "CashRegister.AlreadyOpen"

  Scenario: Record cash movement
    Given a cash register is open for branch "1"
    When I record a cash movement of type "Entry" with amount 500.00
    Then the movement should be recorded successfully
    And the cash register should remain open

  Scenario: Close a cash register
    Given a cash register is open for branch "1"
    When I close the cash register with closing balance 1500.00
    Then the cash register should be closed successfully
    And the status should be "Closed"

  Scenario: Get cash register balance
    Given a cash register with movements totaling 750.00
    When I get the balance for the cash register
    Then the opening balance should be 1000.00
    And the calculated balance should be 1750.00
