# Knotify 🔔
A Windows desktop social media application built in C# as a 
Data Structures & Algorithms mid-semester project. Users can 
create posts, interact with friends, and navigate a full social 
feed — all powered by classic data structures under the hood.

## Overview
Knotify demonstrates how core DSA concepts apply to real-world 
systems. Every major feature maps directly to a data structure — 
the feed runs on a Queue, undo actions use a Stack, friendships 
live in a Linked List, and comments are stored in a Custom Linked 
List. The app is built on a clean four-layer architecture: UI, 
Business Logic, Data, and Data Structures.

## Features
- User authentication — login, signup, and password recovery
- Post feed powered by a Queue (FIFO) for chronological display
- Create, edit, and delete posts with real-time statistics
- Like and comment on posts with linked list tracking
- Undo system (undo like, delete, or navigation) via a Stack
- Friend system — send, accept, and reject friend requests
- Sorted feed — Newest First, Oldest First, and Trending views
- Keyword and username search across the feed
- Stack-based back navigation between screens
- Profile management — edit username, email, and contact info

## Data Structures
| Structure | Used For |
|---|---|
| Queue | Main post feed (FIFO display) |
| Stack | Undo operations & back navigation |
| Linked List | User storage & friend relationships |
| Custom Linked List | Comments and likes per post |

## Architecture
The project follows a four-layer design:
- **UI Layer** — forms, input handling, and visual feedback
- **BL Layer** — business entities, rules, and validation
- **DL Layer** — CRUD operations and algorithm execution
- **Data Structures Layer** — custom DSA implementations

## How to Run
1. Open the solution file in **Visual Studio**
2. Set `DSA_MidProject` as the startup project
3. Ensure a **MySQL** database is configured via `DatabaseHelper`
4. Build and run — the app launches as a Windows desktop window

> Requires Windows and .NET — developed and tested in Visual Studio.

## Performance
- **Best Case:** O(1) for most CRUD operations
- **Average Case:** O(log n) searches, O(n log n) sorting
- **Worst Case:** O(n²) QuickSort (bad pivot), O(n) linear search
- **Space:** O(n) for users, posts, and comments

## Tech Stack
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=flat&logo=mysql&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91?style=flat&logo=visualstudio&logoColor=white)

## About
Built as a mid-semester project for CSC-200L Data Structures and 
Algorithms at the University of Engineering and Technology, Lahore. 
The goal was to implement practical social media functionality 
using Queues, Stacks, and Linked Lists — demonstrating that DSA 
isn't just theory, it's the backbone of real applications.
