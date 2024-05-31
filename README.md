# UniWork

UniWork is a Unity asset designed to integrate Trello with the Unity Editor, allowing developers to manage their Trello boards and cards directly within Unity. It provides an intuitive interface for interacting with Trello, including viewing boards, lists, and cards, as well as editing card details.

**_This asset is still in development._**

## Features

- **Trello Integration**: Access your Trello boards, lists, and cards directly within the Unity Editor.
- **Drag-and-Drop Cards**: Move cards between lists using a drag-and-drop interface.
- **Card Details Popup**: View and edit card details, including the description, attachments, comments, and labels, in a pop-up window.
- **Add New Cards**: Create new cards directly from the Unity Editor.
- **API Key and Token Configuration**: Easily configure your Trello API key and token.

## How It Works

### Installation

1. Clone or download the `UniWork` repository.
2. Place the `UniWork` folder into your Unity project's `Assets` directory.

### Setup Trello Power-Up

1. Go to [Trello Developer Portal](https://trello.com/power-ups/admin).
2. Click on `Create new Power-Up`.
3. Fill in the required information:
    - **Name**: `UniWork`
    - **Workspace**: Select your workspace
    - **Iframe Connector URL**: `https://example.com` (You can set this to any URL for now)
4. Save your Power-Up.
5. Go to the `API Key` section and generate a new API Key. Note it down.
6. In the `OAuth` section, generate an API Token. Note it down.
7. Enable the capabilities your Power-Up needs:
    - `board-buttons`
    - `card-buttons`
    - `card-back-section`
    - `authorization-status`
    - `show-authorization`
    - `callback`

### Usage

1. **Opening UniWork**:
    - Go to `Tools > UniWork` in the Unity menu to open the UniWork window.

2. **Configuring API Key and Token**:
    - Click on the "Settings" button in the UniWork window.
    - Enter your Trello API key and token, then click "Save".

3. **Viewing Boards and Lists**:
    - The left column displays your Trello boards. Click on a board to load its lists and cards.
    - The right column displays the lists and cards for the selected board.

4. **Managing Cards**:
    - Click on a card to open a detailed view in a pop-up window, where you can edit the card's name, description, attachments, comments, and labels.
    - Drag and drop cards between lists to update their position.

5. **Adding New Cards**:
    - Click the "Add Card" button in any list to create a new card. Enter the card's name in the prompt that appears.

### Features in Detail

- **Card Details Popup**:
    - Click on a card to open the details window, where you can:
        - **Edit Name**: Modify the card's name.
        - **Edit Description**: Update the card's description.
        - **Manage Attachments**: View, add, or remove attachments.
        - **Manage Comments**: View, add, or remove comments.
        - **Manage Labels**: View, add, or remove labels.

- **Drag-and-Drop Interface**:
    - Easily move cards between lists by dragging and dropping them.

- **API Key and Token Configuration**:
    - Store your Trello API key and token securely using Unity's `EditorPrefs`.
    - Retrieve and use the stored credentials automatically when UniWork starts.

## Known Issues

- **_This asset is still in development_**
- **_Current limitations and known bugs will be posted here._**

## Changelog

### Version 0.3

- Added functionality to view and edit card descriptions.
- Enhanced the drag-and-drop interface for moving cards between lists.
- Improved the card details popup with additional features for managing attachments, comments, and labels.

### Version 0.2

- Implemented API key and token configuration.
- Added initial support for viewing Trello boards, lists, and cards.
- Enabled adding new cards directly from the Unity Editor.
- Introduced basic drag-and-drop functionality for cards.

### Version 0.1

- Initial release with basic Trello integration.
- Ability to view Trello boards and lists in the Unity Editor.
- Simple UI for interacting with Trello cards.
