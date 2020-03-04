import React, { Component } from "react";
import styled, { ThemeProvider } from "styled-components";
import {
  Switch,
  Redirect,
  Route,
  BrowserRouter as Router
} from "react-router-dom";
import { Provider } from "react-redux";
import theme from "../styles/theme";
import Header from "./Header";
import Navigation from "./Navigation";
import ChatBotContainer from "./ChatBotContainer";
import MainContent from "./MainContent";
import Home from "./Home";
import PermitForm from "./PermitForm";
import Footer from "./Footer";
import Login from "./Login";
import store from "../redux/store";

const Styled = styled.div`
  &.App {
    background-color: white;
    display: grid;
    grid-template-rows: auto 1fr;
    grid-template-columns: auto 1fr;
    min-height: 100vh;

    .Header {
      grid-row: 1 / 2;
      grid-column: 1 / span 2;
    }
    .ChatBot {
      grid-row: 2;
      grid-column: 1;
    }
    .MainContent {
      grid-row: 2;
      grid-column: 2;
    }
  }
`;

class App extends Component {
  constructor(props) {
    super(props);
    // In the URL, when bot=true the bot will be expanded by default
    this.searchParams = new URLSearchParams(window.location.search);
    const isBotExpanded = this.searchParams.get("bot");
    localStorage.setItem("user", '{"name":"David Carmona","image":null}');
    const currentUser = JSON.parse(localStorage.getItem("user"));
    this.state = {
      isBotExpanded: !!isBotExpanded,
      isLoggedIn: !!currentUser,
      user: currentUser,
      displayLogoutButton: false
    };
  }

  toggleLogoutButton = () => {
    this.setState(prevState => {
      return { displayLogoutButton: !prevState.displayLogoutButton };
    });
  };

  onLogout = () => {
    localStorage.removeItem("user");
    window.location.href = "/";
  };

  closeChatBot = () => {
    this.setState({ isBotExpanded: false });
  };

  openChatBot = () => {
    this.setState({ isBotExpanded: true });
  };

  render() {
    const { isBotExpanded, isLoggedIn, user, displayLogoutButton } = this.state;
    return (
      <ThemeProvider theme={theme}>
        <Provider store={store}>
          <Router>
            <Styled className="App">
              <Header
                user={user}
                displayLogoutButton={displayLogoutButton}
                toggleLogoutButton={this.toggleLogoutButton}
              />
              <MainContent>
                <Navigation />
                <Switch>
                  <Route exact path="/" component={Home} />
                  <Route
                    path="/permit-request"
                    render={() => {
                      if (!isBotExpanded) {
                        // For the purposes of this demo, do not let users view the permit form
                        // unless they're interacting with the bot. That means, that if the bot
                        // is not expanded, then we'll redirect users back to the home page.
                        return <Redirect to="/" />;
                      }
                      return <PermitForm user={user} />;
                    }}
                  />
                </Switch>
                <Footer />
              </MainContent>
              <ChatBotContainer
                isExpanded={isBotExpanded}
                handleOpen={this.openChatBot}
                handleClose={this.closeChatBot}
                user={user}
              />
            </Styled>
          </Router>
        </Provider>
      </ThemeProvider>
    );
  }
}

export default App;
