# Backend developer technical test

The test involves creating a .NET Web API application with a REST API to:
- Create expenses
- List expenses

## Specifications
### Resources
#### Expenses
An expense is characterized by:

- A user (person who made the purchase)
- A date
- A type (possible values: Restaurant, Hotel, and Misc)
- An amount and a currency
- A comment

#### Users
A user is characterized by:

- A last name
- A first name
- A currency in which the user makes their expenses

### Main features
#### Creating an expense

This REST API should allow:
 - Creating an expense considering the validation rules.

Expense validation rules:
- An expense cannot have a date in the future,
- An expense cannot be dated more than 3 months ago,
- The comment is mandatory,
- A user cannot declare the same expense twice (same date and amount),
- The currency of the expense must match the user’s currency.

#### Listing expenses
This REST API should allow:

- Listing the expenses for a given user,
- Sorting expenses by amount or date,
- Displaying all the properties of the expense; the user of the expense should appear in the format `{FirstName} {LastName}` (e.g., "Anthony Stark").

### Additional information
- Authentication management is not expected.
- Development of an user interface is not required.

## Technical constraints
### Language
The application must be developed in C#/.NET.

### Storage
Data must be persisted in an SQL database.

The users' table should be initialized with the following users:

- Stark Anthony (with the currency being the U.S. dollar),
- Romanova Natasha (with the currency being the Russian ruble).



### Quality criteria
- The code must be clean, readable, extensible, well-structured, and easily maintainable.
- The code must adhere to best development practices.
- The proposed solution must include unit tests.

### Acceptance criteria
All functionalities described in the instructions must be implemented and functional.

The expense validation rules must be unit tested.

### Performance criteria
- The application must be fast and responsive.
- Loading times must be minimal.

