import React from "react";

const PREVIOUS_KEY = 38; // Up arrow key
const NEXT_KEY = 40; // Down arrow key
const USER_INTERACTIONS = [
  "Tell me a joke",
  "What can you do?",
  "I have a fridge I need to get rid of next Friday",
  "Yes",
  "I want to remodel my kitchen",
  "I don’t know the value yet"
];

export default function withKeyboardSupport(ChatBotComponent) {
  return class extends React.Component {
    interactionIndex = -1;
    state = {
      userInteraction: ""
    };

    componentDidMount() {
      document.addEventListener("keydown", this.keyboardHandler);
    }
    componentWillUnmount() {
      document.removeEventListener("keydown", this.keyboardHandler);
    }

    keyboardHandler = e => {
      const key = e.which;
      if (key === NEXT_KEY) {
        // Up arrow adds keyboard support to the chat bot
        this.updateInteraction(1);
      } else if (key === PREVIOUS_KEY) {
        this.updateInteraction(-1);
      }
    };

    updateInteraction = offset => {
      let index = this.interactionIndex + offset;
      this.interactionIndex = index > 0 ? index % USER_INTERACTIONS.length : 0;
      const userInteraction = USER_INTERACTIONS[this.interactionIndex];
      this.setState({ userInteraction });
    };

    render() {
      return (
        <ChatBotComponent
          userInteraction={this.state.userInteraction}
          {...this.props}
        />
      );
    }
  };
}
