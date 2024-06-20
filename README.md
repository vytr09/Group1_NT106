# iConnect - Intelligent Connect

Chat and connect with your friends and family seamlessly.

## Description

[iConnect](https://www.taskade.com/d/6K8rPuB4FFmJ68n2?from=shared) is a network programming application developed by Group 1 from Class NT106 at the University of Information Technology. It enables users to seamlessly chat and connect with their friends and family through an intuitive interface and intelligent features.

## Features

- Real-time messaging
- Newsfeed
- User authentication
- Notifications
- User-friendly interface with Guna2UI components

## Getting Started

### Prerequisites

Before you begin, ensure you have met the following requirements:
- You have installed [Visual Studio](https://visualstudio.microsoft.com/) with .NET Framework support.
- You have a Firebase account and a Realtime Database set up. Instructions can be found [here](https://firebase.google.com/docs/database).
- You have [Git](https://git-scm.com/) installed.

### Installation

To install iConnect, follow these steps:

1. **Clone the repository**:
    ```bash
    git clone https://github.com/tranthehuuphuc/Group1_NT106.git
    ```

2. **Open the project**:
    Open the cloned repository in Visual Studio.

3. **Configure Firebase**:
    - Add your Firebase Realtime Database URL and credentials to the configuration file. You may need to install Firebase dependencies using NuGet Package Manager in Visual Studio.
    - To do this, open NuGet Package Manager, search for `FirebaseDatabase.net`, and install it.

4. **Install Dependencies**:
    - Install Guna.UI2.WinForms via NuGet Package Manager:
      ```bash
      Install-Package Guna.UI2.WinForms
      ```

5. **Build the project**:
    - In Visual Studio, build the solution by clicking on `Build` > `Build Solution`.

6. **Run the application**:
    - Start the application by clicking `Start` or pressing `F5`.

## Usage

### Running the Application

After installing and running the application, you can start chatting and connecting with your friends and family. The user interface is designed to be intuitive, making it easy to send messages, create chat groups, and manage contacts.

### User Guide

1. **Login/Signup**: Create an account or log in using your existing credentials.
2. **Newsfeed**: Post something on your newsfeed to express yourself.
3. **Start a Chat**: Select a contact and start a chat. You can send text messages, images, and files.
4. **Notifications**: Receive real-time notifications for new messages.

## Technology Stack

- **Programming Language**: C#
- **Framework**: .NET Framework
- **Database**: Firebase Realtime Database
- **User Interface**: Windows Forms with Guna2UI

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

To contribute to iConnect, follow these steps:

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Authors

### Contributors and Contact Info

- **Tran The Huu Phuc**: [@tranthehuuphuc](https://github.com/tranthehuuphuc)
- **Tran Thi Thuy Vy**: [@vytr09](https://github.com/vytr09)
- **Le Thi Bich Tuyen**: [@tuyen2201](https://github.com/tuyen2201)
- **Nguyen Nhat Quang**: [@NhatWoan-20](https://github.com/NhatWoan-20)

## Version History

* Initial release

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

Special thanks to our instructors, classmates, and the open-source community for their support and contributions.

## FAQs

### How do I set up Firebase for this project?

You need to create a Firebase project and set up a Realtime Database. Add your Firebase project configuration details to the application's configuration file. More detailed instructions can be found in the [Firebase documentation](https://firebase.google.com/docs/database).

### How can I report an issue or request a feature?

You can report issues and request features by opening an issue in the [GitHub Issues](https://github.com/tranthehuuphuc/Group1_NT106/issues) section of the repository.

