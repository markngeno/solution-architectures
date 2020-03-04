import React, { Component } from "react";
import { connect } from "react-redux";
import styled, { withTheme } from "styled-components";
import { withRouter } from "react-router";
import {
    renderWebChat,
    createDirectLine,
    createStore
    // createCognitiveServicesWebSpeechPonyfillFactory,
} from "botframework-webchat";
import config from "../common/config";
import withKeyboardSupport from "./withKeyboardSupport";
const Styled = styled.div`
  &.ChatBot {
    > div {
      > div:first-child ul {
        /* Max height for activity list */
        max-height: calc(
          100vh - ${props => props.theme.height_navbar}px -
            ${props => props.theme.height_bot_close}px -
            ${props => props.theme.height_bot_send_box}px
        );

        ul {
          max-height: none;

          li .ac-container img {
            /* SMAR-55: Set height on slider images to make slider
               scroll down completely */
            height: 150px;
          }
        }
      }
      > div:last-child ul {
        /* Align suggested actions with bot bubble */
        margin-left: 50px;
      }
    }
    .avatar {
      font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
        sans-serif;
      font-size: 14px;
      color: ${props => props.theme.color_avatar_text};
      background-color: ${props => props.theme.color_avatar_bot};

      &.from-user {
        background-color: ${props => props.theme.color_avatar_user};
      }
    }
    .bubble {
      line-height: 1.4;
    }
  }
`;

function eventToPromise(target, name) {
  return new Promise((resolve, reject) => {
    const handler = event => {
      target.removeEventListener(name, handler);
      resolve(event);
    };

    target.addEventListener(name, handler);
  });
}

async function downscaleImage(imageURL, contentType, maxSize) {
  const image = document.createElement('img');
  const imageLoadPromise = eventToPromise(image, 'load');

  image.src = imageURL;

  await imageLoadPromise;

  const canvas = document.createElement('canvas');

  if (image.height < maxSize && image.width < maxSize) {
    canvas.height = image.height;
    canvas.width = image.width;
  } else {
    canvas.height =
      image.height > image.width
        ? maxSize
        : ~~((image.height / image.width) * maxSize);
    canvas.width =
      image.width > image.height
        ? maxSize
        : ~~((image.width / image.height) * maxSize);
  }
  canvas.getContext('2d').drawImage(image, 0, 0, canvas.width, canvas.height);

  const dataURL = canvas.toDataURL(contentType);

  return dataURL;
}

const dispatchRedirect = (history) => {
  let thumbnails = null;
  return () => next => async action => {
    if(action.type === 'DIRECT_LINE/INCOMING_ACTIVITY') {
      const { activity } = action.payload;
      let { id, text, type, from, name, attachments, channelData } = activity;

      const event = new Event('webchatincomingactivity');
      event.data = activity;
      window.dispatchEvent(event);

      if (
          activity.type === "event" &&
          activity.from.role === "bot" &&
          activity.name === "fill-form"
      ) {
          history.push("/permit-request");
      }
      if (
          activity.type === "event" &&
          activity.from.role === "bot" &&
          activity.name === "go-back"
      ) {
          history.go(-1);
      }

      if (attachments && attachments.length > 0 && channelData) {
        action.payload.activity.attachments = activity.attachments.map(
          attachment => {
            attachment.contentUrl = thumbnails;
            return {
              ...attachment,
              contentUrl: attachment.contentUrl,
            };
          }
        );
      }
    } else if (action.type === 'DIRECT_LINE/POST_ACTIVITY') {
      action.test = 'test';
      if (action.payload.activity && action.payload.activity.attachments) {
        const downscaledImages = await Promise.all(
          action.payload.activity.attachments.map(
            ({ contentType, contentUrl }) =>
              /^image\//.test(contentType)
                ? downscaleImage(contentUrl, contentType, 480)
                : null
          )
        );
        (
          action.payload.activity.channelData ||
          (action.payload.activity.channelData = {})
        ).thumbnails = downscaledImages;
        thumbnails = downscaledImages;
      }
    }

        return next(action);
    };
};


const getInitials = name => {
    const [firstName, lastName] = name.split(" ");
    const firstLetter = firstName.charAt(0);
    const secondLetter = lastName ? lastName.charAt(0) : "";
    return firstLetter + secondLetter;
};

export class ChatBot extends Component {
    isScrolling = false;
    activityListEl = null;

    constructor(props) {
        super(props);
        this.chatBot = React.createRef();
        this.webChatStore = createStore({}, dispatchRedirect(props.history));
    }

    async componentDidMount() {
        const directLine = createDirectLine({
            secret: "<your-direct-line-secret>",
            webSocket: false
        });
        const { theme, user } = this.props;
        const botAvatarInitials = getInitials(config.BOT_NAME);
        const userAvatarInitials = getInitials(user.name);

        renderWebChat(
            {
                directLine,
                store: this.webChatStore,
                userID: "John Doe"
            },
            this.chatBot.current
        );
        document.addEventListener("scroll", this.handleScroll);
    }

    componentDidUpdate(prevProps) {
        const { userInteraction, needsCorrections, isSubmitted, isFormInactive } = this.props;
        if (userInteraction && userInteraction !== prevProps.userInteraction) {
            this.inputFocus();
            this.webChatStore.dispatch({
                type: "WEB_CHAT/SET_SEND_BOX",
                payload: { text: userInteraction }
            });
        } else if (isSubmitted && isSubmitted !== prevProps.isSubmitted) {
            this.webChatStore.dispatch({
                type: "DIRECT_LINE/POST_ACTIVITY",
                payload: {
                    activity: {
                        type: "",
                        name: ""
                    }
                }
            });
        } else if (needsCorrections && needsCorrections.length &&
            needsCorrections !== prevProps.needsCorrections
        ) {
            this.webChatStore.dispatch({
                type: "DIRECT_LINE/POST_ACTIVITY",
                payload: {
                    activity: {
                        type: "",
                        name: "",
                        value: needsCorrections
                    }
                }
            });
        } else if (isFormInactive && isFormInactive !== prevProps.isFormInactive) {
            this.webChatStore.dispatch({
                type: "DIRECT_LINE/POST_ACTIVITY",
                payload: {
                    activity: {
                        type: "",
                        name: ""
                    }
                }
            })
        }
    }

    componentWillUnmount() {
        document.removeEventListener("scroll", this.handleScroll);
    }

    handleScroll = () => {
        const { theme } = this.props;
        if (!this.isScrolling) {
            // Cache activity list element for subsequent calls
            this.activityListEl =
                this.activityListEl ||
                this.chatBot.current.querySelector(
                    "div:first-child > div:first-child ul"
                );

            if (this.activityListEl) {
                // Make sure activity list element exists in the DOM
                window.requestAnimationFrame(() => {
                    const currentScroll = document.documentElement.scrollTop;
                    const navBarHeight = Math.max(theme.height_navbar - currentScroll, 0);
                    const elementsHeight =
                        navBarHeight + theme.height_bot_close + theme.height_bot_send_box;
                    this.activityListEl.style.maxHeight = `calc(100vh - ${elementsHeight}px)`;
                    this.isScrolling = false;
                });
                this.isScrolling = true;
            }
        }
    };

    inputFocus = () => {
        const inputEl = this.chatBot.current.querySelector("form input");
        if (inputEl) {
            inputEl.focus();
        }
    };

    render() {
        return <Styled className="ChatBot" ref={this.chatBot} />;
    }
}

export default withKeyboardSupport(
    withTheme(
        withRouter(
            connect(state => ({
                needsCorrections: state.needsCorrections,
                isSubmitted: state.isSubmitted,
                isFormInactive: state.isFormInactive
            }))(ChatBot)
        )
    )
);
