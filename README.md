# Expenses API

## Description

L'API **Expenses API** est une application .NET Core permettant de gérer les dépenses des utilisateurs. Elle permet :

- La création de dépenses associées aux utilisateurs, tout en validant certaines règles de gestion.
- La récupération des dépenses par utilisateur, avec des options de tri.
- Une structure de données simple et intuitive pour gérer les utilisateurs et leurs dépenses.

## Table des matières

1. Prérequis
2. Installation et configuration
3. Démarrage de l'application
4. Architecture de l'application
5. Endpoints de l'API
6. Tests

## 1. Prérequis

- .NET 6 SDK installé sur votre machine.
- Utilisation de SQLite
- Un client HTTP pour tester les endpoints, comme Postman ou Swagger.

## 2. Installation et configuration

#### Clonez le dépôt :

```sh
git clone <URL>
cd ExpensesAPI
```

#### Configurer la base de données :

```sh
"ConnectionStrings": {
  "DefaultConnection": "Data Source=expenses.db"
```

#### Installer les dépendances :

```sh
dotnet restore
```

#### Appliquer les migrations et initialiser la base de données :

```sh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 3. Démarrage de l'application

#### Pour lancer le serveur :

```sh
dotnet run
```

L'API sera accessible sur https://localhost:5112.

Swagger est également disponible pour explorer et tester l'API :

Accédez à https://localhost:5112/swagger pour voir la documentation Swagger de l'API.


## 4. Architecture de l'application

L'application suit une architecture Layered Architecture avec une séparation claire des responsabilités :

- **Domain** : Contient les classes de base de l'application, telles que User et Expense.
- **Data** : Contient la configuration du contexte de la base de données (AppDbContext).
- **Repositories** : Contient les interfaces et implémentations pour accéder et manipuler les données (UserRepository, ExpenseRepository).
- **Services** : Contient la logique métier (Business Logic) dans ExpenseService, qui valide et traite les données avant de les envoyer aux contrôleurs.
- **Controllers** : Contient les points d'entrée API (ExpensesController) exposés aux clients.

## 5. Endpoints de l'API

### Endpoints

- POST	/api/expenses	Créer une nouvelle dépense
- GET	/api/expenses/{userId}	Récupérer les dépenses d'un utilisateur avec des options de tri

### Exemples de Requête

Création d'une dépense
Requête :

```json
{
  "date": "2024-10-27T10:00:00",
  "type": 1,
  "amount": 150,
  "currency": 0,
  "comment": "Business lunch",
  "user": {
    "firstName": "Natasha",
    "lastName": "Romanova",
    "currency": 0
  }
}
```

```json
{
  "message": "Expense created successfully."
}
```

### Récupération des dépenses d'un utilisateur

Il suffit de compléter les informations telles que *userId*, *sortby* (amount ou date) et *sortOrder* (asc ou desc)

Exemple de réponses
```json
[
  {
    "date": "2024-10-27T10:00:00",
    "type": "Hotel",
    "amount": 150,
    "currency": "USD",
    "comment": "Business lunch",
    "userFullName": "Natasha Romanova"
  }
]
```

## 6. Tests

Les tests sont organisés dans les dossiers ExpensesAPI.Tests avec des tests unitaires pour les services et les contrôleurs.

Exécution des tests
Restaurer les dépendances de test :

```sh
dotnet restore
dotnet test
```
