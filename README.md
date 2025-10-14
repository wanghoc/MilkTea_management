# MilkTeaShop — WPF + OOP + Design Patterns (Skeleton)

This is a **ready-to-fill** skeleton for your Design Pattern project.

## Structure
- `MilkTeaShop.Presentation` (WPF, MVVM) — UI layer
- `MilkTeaShop.Application` — application services (Order, Pricing, Promotion, Inventory)
- `MilkTeaShop.Domain` — entities, value objects, design patterns (Builder, Decorator, Strategy, State)
- `MilkTeaShop.Infrastructure` — repositories (in-memory stubs), adapters (Payment/Printer)

## Open & Run
1. Open `MilkTeaShop.sln` with Visual Studio 2022 (or newer).
2. Set `MilkTeaShop.Presentation` as **Startup Project**.
3. Press **F5** to run (you will see a minimal POS screen).

> Notes
- The Infrastructure project uses **in-memory** stubs to avoid NuGet dependencies. You can switch to EF Core later.
- WPF requires **.NET 8 SDK + Desktop workload**. TargetFramework: `net8.0-windows`.

## Where to put your code
- Domain patterns:
  - Builder: `Domain/Patterns/Builder`
  - Decorator: `Domain/Patterns/Decorator`
  - Strategy (Promotion/Pricing): `Domain/Patterns/Strategy`
  - State (Order lifecycle): `Domain/Patterns/State`
- Add new features by extending **Application Services** and **ViewModels**.
