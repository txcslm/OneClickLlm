# VIPER Architecture Example

This document sketches how the OneClick LLM application could look if implemented with the VIPER architectural pattern. VIPER divides responsibilities into five components:

- **View** handles user interface elements.
- **Interactor** contains business logic and communicates with services.
- **Presenter** coordinates between the View and the Interactor.
- **Entity** defines simple model data structures.
- **Router** manages navigation between screens.

Below is a minimal example illustrating these roles in C#.

```csharp
// Entities/Message.cs
namespace OneClickLlm.Viper.Entities;
public record Message(string Role, string Text);

// Interactors/IChatInteractor.cs
namespace OneClickLlm.Viper.Interactors;
public interface IChatInteractor
{
    Task<IEnumerable<Message>> SendAsync(string prompt);
}

// Presenters/IChatPresenter.cs
namespace OneClickLlm.Viper.Presenters;
public interface IChatPresenter
{
    void OnPromptSubmitted(string prompt);
}

// Views/IChatView.cs
namespace OneClickLlm.Viper.Views;
public interface IChatView
{
    event Action<string>? PromptSubmitted;
    void ShowMessages(IEnumerable<Message> messages);
}

// Router/IChatRouter.cs
namespace OneClickLlm.Viper.Router;
public interface IChatRouter
{
    void ShowModelSelection();
}
```

Each component interacts only with its immediate neighbors. The view raises events to the presenter, which invokes the interactor to perform operations. The router abstracts navigation logic. This pattern enforces strict separation of concerns and keeps user interface logic independent from business rules.
