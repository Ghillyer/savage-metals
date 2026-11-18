# Savage Metals

An inventory app for a metal processing shop. You buy coils, process them into new pieces, sell them, and always know what the metal on hand cost.

Built with a .NET 10 Web API, SQL Server, React + TypeScript, Material UI, and Firebase sign-in. Hosted on Azure.

## Try it

- **App:** https://blue-flower-054637910.6.azurestaticapps.net
- **API:** https://savage-metals-dgakgyczhycyaqbm.centralus-01.azurewebsites.net

There are two demo accounts, an **Admin** and a **Purchaser**. The credentials are included with my submission.

The app runs on free Azure tiers that go to sleep when they're not being used. The first load can take 20 to 60 seconds. After that it's fast.

### What to look at

The live app already has a full example entered:

1. **PO 2201** buys a hot-rolled steel coil and aluminum sheet from Midwest Steel Supply for $23,550. The coil is tagged C-1001 in Bay 2, and the aluminum is in Rack 1.
2. **WO-1001** slits C-1001 (40,000 lb, $16,800) into four 9,900 lb slit coils at $4,200 each in Rack 4, plus 400 lb of scrap at $0 in the Scrap Bin.
3. **SO 5001** ships two slit coils to Great Lakes Fabrication for $10,890. Cost was $8,400, so profit is $2,490 (22.9% margin).
4. **Reports** show $15,150 still on hand: $23,550 bought minus $8,400 sold.

Sign in as the Purchaser to see the same app without the admin pages.

## How it works

The app follows metal through the shop:

1. **Buy.** Create a purchase order with the product, grade, weight, and price per lb on each line.
2. **Receive.** When the metal shows up, give each line a tag number and a location. Each line becomes a tagged item in inventory.
3. **Process.** A work order takes one item (like a big coil) and turns it into new tagged items (like smaller slit coils, plus scrap).
4. **Sell.** A sales order sells available items to a customer and shows the revenue, cost, profit, and margin.
5. **Report.** See what's on hand by product and by location, and the profit on each sale.

### Cost follows the metal

Every tagged item carries its own cost.

- When a PO is received, each item's cost is its line total.
- When a work order runs, the input's cost is split across the new pieces **by weight**. Scrap gets $0. The pieces always add up to exactly what the input cost.
- When an item is sold, its cost is what the sale is measured against.

So the value of inventory only changes when metal is bought or sold. Processing just moves the cost onto the new pieces.

### Numbering

- **Numbers that come from outside the shop are typed in:** tag numbers, PO numbers, SO numbers, and container numbers.
- **Numbers that only exist inside the app are generated:** work orders are numbered `WO-1001`, `WO-1002`, and so on.
- To save typing, the PO number is filled in with the next number. It can still be changed.

### Who can do what

- **Purchasers** create and receive POs, create work orders and sales orders, and view reports.
- **Admins** can do all of that, plus manage products, locations, vendors, and users.

Admin pages are hidden from Purchasers, and the API blocks them too. Purchasers can see margins on purpose, because they need them to price sales.

## Features

- Purchase orders with receiving
- Work orders with automatic cost splitting
- Sales orders with profit and margin
- Inventory and sales reports
- Admin pages for products, locations, vendors, and users
- Inviting users by email (they get an email to set their password)
- Deactivating records instead of deleting them, so history stays intact

## Nice touches

- The first field is selected when a dialog opens, so you can start typing right away.
- The PO number is filled in for you.
- Picking a product fills in its price.
- Totals update as you type.
- Work orders show how much weight is left to assign.
- You can only pick items that are available, and you can't add the same item twice.
- Errors explain what went wrong, like "Tag C-1001 is already in use."
- It works on a tablet in portrait mode without tables getting squished.
- Refreshing any page keeps you where you were.
- A dark theme to match the Savage Innovations brand.

## Built with

- **API:** .NET 10, Dapper, SQL Server
- **Database changes:** FluentMigrator
- **Sign-in:** Firebase Authentication
- **Frontend:** React, TypeScript, Vite, Material UI, TanStack Query
- **Hosting:** Azure Static Web Apps (app), Azure App Service (API), Azure SQL (database)

## A few design choices

- **Simple layers.** Controllers handle the web requests, services hold the business rules, and repositories talk to the database.
- **Plain SQL with Dapper.** The queries are written out by hand, so it's clear exactly what runs against the database.
- **Safe status changes.** An item can't be processed or sold twice, even if two people try at the same moment. The second one gets a clear message instead.
- **Firebase for sign-in, the app's own database for roles.** Firebase confirms who you are. The app decides what you can do, so deactivating a user takes effect right away.
- **Clear errors.** Bad input, missing records, and conflicts each return a consistent error the app can show to the user.

## API endpoints

Every endpoint requires sign-in. The ones marked **(admin)** are Admin only.

**Current user**
- `GET /api/users/me`: who's signed in

**Users**
- `GET /api/users` (admin)
- `POST /api/users`: invite a user (admin)
- `PUT /api/users/{id}`: change name, role, or active status (admin)

**Products**
- `GET /api/products`
- `GET /api/products/{id}`
- `POST /api/products` (admin)
- `PUT /api/products/{id}` (admin)

**Locations**
- `GET /api/locations`
- `GET /api/locations/{id}`
- `POST /api/locations` (admin)
- `PUT /api/locations/{id}` (admin)

**Vendors**
- `GET /api/vendors`
- `GET /api/vendors/{id}`
- `POST /api/vendors` (admin)
- `PUT /api/vendors/{id}` (admin)

**Purchase orders**
- `GET /api/purchase-orders`
- `GET /api/purchase-orders/next-number`: the suggested next PO number
- `GET /api/purchase-orders/{id}`
- `POST /api/purchase-orders`
- `PUT /api/purchase-orders/{id}`: update an open PO
- `POST /api/purchase-orders/{id}/receive`

**Work orders**
- `GET /api/work-orders`
- `GET /api/work-orders/{id}`
- `POST /api/work-orders`

**Sales orders**
- `GET /api/sales-orders`
- `GET /api/sales-orders/{id}`
- `POST /api/sales-orders`

**Inventory and reports**
- `GET /api/inventory`: items, optionally filtered by status
- `GET /api/reports/inventory`: what's on hand, by product and by location

## Project structure

```
savage-metals/
├── api/                        .NET 10 Web API
│   ├── Controllers/            HTTP endpoints
│   ├── Core/
│   │   ├── DTOs/               Request/response contracts per feature
│   │   ├── Enums/              InventoryStatus, MaterialGrade, PurchaseOrderStatus, UserRole, VendorType
│   │   ├── Exceptions/         InvalidRequest, NotFound, Conflict
│   │   ├── Interfaces/         Service and infrastructure contracts
│   │   ├── Models/             Database row models
│   │   └── Repositories/       Repository contracts
│   ├── Services/               Business rules
│   ├── DAL/                    Dapper repositories (SQL, transactions)
│   ├── Security/               Firebase JWT events, CallerContext, policies
│   ├── Middleware/             GlobalExceptionHandler
│   ├── Migrations/             FluentMigrator migrations
│   ├── Factories/              SQL connection factory
│   ├── Settings/               Firebase and CORS options
│   └── Converters/             UTC DateTime JSON converter
├── migrations/                 Console runner for the migrations
├── app/                        React + TypeScript + Vite
│   ├── public/                 staticwebapp.config.json, icons
│   └── src/
│       ├── auth/               AuthProvider, login, RequireAuth, RequireAdmin
│       ├── components/         Layout and shared components
│       ├── features/           One folder per feature: pages, dialogs, query hooks
│       ├── lib/                axios client, Firebase, formatting, error helpers
│       ├── types/              TypeScript API types
│       └── theme.ts            MUI theme
└── .github/workflows/          Azure Static Web Apps build and deploy
```

## Running it locally

You'll need the .NET 10 SDK, Node.js, SQL Server running on `localhost`, and a Firebase project with email/password sign-in turned on.

**1. Create the database**

```powershell
sqlcmd -S localhost -E -Q "CREATE DATABASE SavageMetals"
```

**2. Set up the tables**

```powershell
dotnet run --project migrations -- 1
```

This creates the tables and two demo users. The demo users are tied to my Firebase project, so with your own project, add yourself as an admin:

```powershell
sqlcmd -S localhost -E -d SavageMetals -Q "INSERT INTO Users (FirebaseUid, Email, DisplayName, Role, IsActive) VALUES ('<your-firebase-uid>', '<your-email>', 'Local Admin', 1, 1)"
```

**3. Start the API**

```powershell
dotnet user-secrets set "Firebase:ServiceAccountPath" "C:\path\to\service-account.json" --project api
dotnet user-secrets set "Firebase:ProjectId" "<your-firebase-project-id>" --project api
dotnet run --project api --launch-profile http
```

The API runs at `http://localhost:5189`.

**4. Start the app**

```powershell
cd app
Copy-Item .env.example .env.local
```

Fill in `.env.local` with your Firebase web app settings, and set `VITE_API_URL=http://localhost:5189/api`. Then:

```powershell
npm install
npm run dev
```

Open `http://localhost:5173`.

## How it's deployed

- **The app** is on Azure Static Web Apps. Every push to `main` rebuilds and redeploys it automatically through GitHub Actions.
- **The API** is on Azure App Service.
- **The database** is Azure SQL, set up with the same migration runner as local.
- **Secrets** (the database connection and the Firebase key) are stored in Azure settings, never in the code.

## Assumptions

- A work order processes one item at a time.
- Items are sold whole, not by partial weight.
- A PO is received all at once.
- Tag numbers are never reused.
- Work orders and sales orders can't be edited after they're created.
- Weights are in pounds and money is in US dollars.

## AI usage

I used AI heavily on this project, mainly Claude and OpenAI's Astras 6. They helped plan the architecture, generate code, explain options and tradeoffs, and review the work. I directed the decisions, made the changes in the codebase, tested each feature through the API and the UI before committing, and deployed and verified the app on Azure.

## What I skipped

I skipped the optional extras (photos on POs, barcode/QR labels, and offline support) to focus on getting the core workflow right, the numbers correct, and the app easy to use on an iPad within the week.

## What I'd do next

- **Editing orders.** Allow changes to purchase orders, work orders, and sales orders where it's safe, like before a PO is received or before an item has moved on to another order.
- **Traceability report.** Follow any piece of metal from the PO it was bought on, through every work order that processed it, to the sales order it shipped on.
- **Profit and loss reports.** Profit by month, by year, or for any date range.
- **Purchaser performance.** Margin on the material each purchaser bought, to show who's buying well.
- **PDF exports.** Download reports and orders as PDFs to share or print.