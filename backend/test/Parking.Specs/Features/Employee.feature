Feature: Employee Management
  As a system user
  I want to manage employees
  So that I can track employee information and schedules

  Scenario: Create a new employee
    Given I have valid employee data
    When I create an employee with CPF "12345678901" and name "John Doe"
    Then the employee should be created successfully
    And the employee status should be active

  Scenario: Cannot create employee with duplicate CPF
    Given an employee already exists with CPF "12345678901"
    When I try to create another employee with the same CPF "12345678901"
    Then the creation should fail with error "Employee.DuplicateCPF"

  Scenario: Assign schedule to employee
    Given an employee with ID "1" exists
    When I assign a schedule for DayOfWeek 1 from 08:00 to 17:00
    Then the schedule should be created successfully
    And the employee should have the schedule assigned

  Scenario: Generate payroll for employee
    Given an employee with ID "1" exists
    When I generate payroll for month "2026-01" with base salary 5000.00
    Then the payroll should be created with status "Draft"
    And the total amount should be calculated correctly
