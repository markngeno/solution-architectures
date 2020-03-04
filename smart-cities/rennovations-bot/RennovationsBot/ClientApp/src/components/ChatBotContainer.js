import React, { PureComponent } from "react";
import styled, { withTheme } from "styled-components";
import ChatBot from "./ChatBot";
import CloseIcon from "../icons/close_icon";
import SmartCityIcon from '../icons/smart-city-bot_icon';

const Styled = styled.div`
  --bot-window-width: 400px;
  --bot-slide-time: 0.6s;

  &.ChatBotContainer {
    position: relative;
    background-color: white;
    box-shadow: 8px 0px 25px -1px rgba(0, 0, 0, 0.35);
    transition: width var(--bot-slide-time);

    .bot-container {
      position: fixed;
      top: 0;
      width: var(--bot-window-width);
      transition: left var(--bot-slide-time);
      height: 100vh;

      .bot-close {
        position: absolute;
        top: 50px;
        width: var(--bot-window-width);
        background-color: #e1e1e1;
        color: ${props => props.theme.color_tertiary};
        height: ${props => props.theme.height_bot_close}px;
        z-index: 5;

        .SmartCityIcon {
          width: 46px;
          height: 30px;
          margin-top: 11px;
          margin-left: 11px;

          path {
            fill: ${props => props.theme.color_tertiary} !important;
          }
        }
        span {
          display: inline-block;
          font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
            sans-serif;
          margin-left: 18px;
          vertical-align: 8px;
        }
        button {
          width: 1rem;
          height: 1rem;
          position: absolute;
          top: 18px;
          right: 15px;

          .CloseIcon {
            width: 0.75rem;
            height: 0.75rem;
            path {
              fill: #8a8d90 !important;
            }
          }
        }
      }

      > div:last-child {
        position: absolute;
        width: 100%;
        bottom: 0;
      }

      .main.modified {
        height: ${props => props.theme.height_bot_send_box}px;

        form input {
          margin: 12px 0;
          height: 38px;
          background-color: #e1e1e1;
          border-radius: 20px;
          padding-left: 14px;
          color: #2b2c32;
        }

        #bot-plus-button {
          height: 100%;
          width: 2rem;
          margin: 0 10px;
        }
      }
    }

    &.expanded {
      width: var(--bot-window-width);

      .bot-container {
        left: 0;

        > div {
          left: 0;
        }
      }
    }

    &.collapsed {
      width: 0;

      .bot-container {
        left: calc(var(--bot-window-width) * -1);

        > div {
          left: calc(var(--bot-window-width) * -1);
        }
      }
    }

    .bot-button {
      position: fixed;
      bottom: 0;
      left: 30px;
      color: white;
      background: ${props => props.theme.blue_gradient};
      height: 40px;
      border-top-left-radius: 2px;
      border-top-right-radius: 2px;

      svg {
        width: 46px;
        height: 30px;
        margin: 5px 0 2px 12px;
      }
      span {
        display: inline-block;
        font-size: 1.125rem;
        margin: 0 18px 0 25px;
        vertical-align: 8px;
      }

      &.visible {
        transition: height 0.3s;
        transform: translateY(0);
        > * {
          transition: opacity 0.2s ease 0.3s;
          opacity: 1;
        }
      }
      &.hidden {
        height: 0;
        transition: height 0.4s ease 0.3s;
        > * {
          transition: opacity 0.3s;
          opacity: 0;
        }
      }
    }

    #bot-attach-icon,
    #bot-mic-icon,
    #bot-plus-icon {
      height: 2rem;
      width: 2rem;
    }
  }
`;

class ChatBotContainer extends PureComponent {
    observer = null;
    isScrolling = false;

    constructor(props) {
        super(props);
        this.containerEl = React.createRef();
        this.closeEl = React.createRef();
        this.state = {
            isBotInit: false
        };
    }

    customizeBotControls = botEl => {
        //const botControls = botEl.querySelector(".main");
        //if (botControls && !botControls.classList.contains("modified")) {
        //  // Copy custom bot control icons
        //  const attachIcon = document.getElementById("bot-attach-icon");
        //  const micIcon = document.getElementById("bot-mic-icon");
        //  const plusButton = document.getElementById("bot-plus-button");
        //  const attachIconDup = attachIcon.cloneNode(true);
        //  const micIconDup = micIcon.cloneNode(true);
        //  const plusButtonDup = plusButton.cloneNode(true);

        //  // Insert/replace custom bot icons into the DOM
        //  const attachmentControl = botControls.childNodes[0];
        //  const defaultAttachmentIcon = attachmentControl.querySelector("svg");
        //  const defaultMicIcon = botControls.childNodes[2].querySelector("svg");
        //  defaultAttachmentIcon.replaceWith(attachIconDup);
        //  defaultMicIcon.replaceWith(micIconDup);

        //  // Rearrange order of bot controls
        //  botControls.insertBefore(plusButtonDup, attachmentControl);
        //  botControls.appendChild(attachmentControl);
        //  botControls.classList.add("modified");
        //}
    };

    handleScroll = () => {
        const { isExpanded, theme } = this.props;
        if (!this.isScrolling && isExpanded) {
            window.requestAnimationFrame(() => {
                const currentScroll = document.documentElement.scrollTop;
                const closeTop = Math.max(theme.height_navbar - currentScroll, 0);
                this.closeEl.current.style.top = closeTop + "px";
                this.isScrolling = false;
            });
            this.isScrolling = true;
        }
    };

    componentDidCatch(error, info) {
        console.log("UI Error: ", error, info);
    }

    componentDidMount() {
        const chatEl = this.containerEl.current;

        // Callback function to execute when mutations are observed
        // i.e. when new DOM nodes are inserted for the bot
        const callback = () => {
            this.customizeBotControls(chatEl);
        };

        this.observer = new MutationObserver(callback);
        this.observer.observe(chatEl, { childList: true, subtree: true });
        document.addEventListener("scroll", this.handleScroll);
    }

    componentDidUpdate(prevProps) {
        const { isExpanded } = this.props;
        if (isExpanded !== prevProps.isExpanded && isExpanded) {
            this.handleScroll();

            if (!this.state.isBotInit) {
                // The first time the chat bot panel is expanded,
                // the bot will finally be rendered into the DOM
                this.setState({ isBotInit: true });
            }
        }
    }

    componentWillUnmount() {
        this.observer.disconnect();
        document.removeEventListener("scroll", this.handleScroll);
    }

    render() {
        const { isBotInit } = this.state;
        const { isExpanded, handleOpen, handleClose, user } = this.props;
        const botClasses =
            "ChatBotContainer" + (isExpanded ? " expanded" : " collapsed");
        const buttonClasses = "bot-button" + (!isExpanded ? " visible" : " hidden");
        return (
            <Styled className={botClasses}>
                <div className="bot-container" ref={this.containerEl}>
                    {/* Instead of creating these icons dynamically
                to replace the default bot controls each time
                they're rendered, they're added to the DOM
                and copied over to replace the default bot
                controls when needed */}
                    <div style={{ display: "none" }}>
                    </div>
                    <div className="bot-close" ref={this.closeEl}>
                        {/* Add bot's header logo and name here */}
                        <button onClick={handleClose}>
                            <CloseIcon className="CloseIcon" />
                        </button>
                    </div>
                    {isBotInit && <ChatBot user={user} />}
                </div>
                <button className={buttonClasses} onClick={handleOpen}>
                    <SmartCityIcon />
                    <span>Smart City Agent</span>
                </button>
            </Styled>
        );
    }
}

export default withTheme(ChatBotContainer);
