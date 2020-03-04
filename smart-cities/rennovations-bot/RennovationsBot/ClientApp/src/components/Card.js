import React from "react";
import styled from "styled-components";
import CaretIcon from "../icons/caret_icon";
import GradientRibbon from "./GradientRibbon";

const Styled = styled.section`
  &.Card {
    position: relative;
    padding: 185px 15px 0 30px;
    color: ${props => props.theme.color_text};
    box-shadow: ${props => props.theme.box_shadow};

    b {
      font-family: SegoeUISemilight, "Helvetica Neue", Helvetica, Arial,
        sans-serif;
      font-weight: normal;
      font-size: 1.5rem;
    }
    p {
      line-height: 1.2;
      padding-right: 20px;
      margin-top: 10px;
    }

    button {
      display: block;
      text-transform: uppercase;
      margin: 25px 0;
      color: ${props => props.theme.color_link};

      .CaretIcon {
        height: 1rem;
      }
      > span {
        vertical-align: 2px;
        margin-left: 9px;
      }

      &:hover {
        cursor: pointer;
      }
    }
  }
`;

const Card = ({ children, imgUrl }) => (
  <Styled
    className="Card"
    style={{ background: `white url(${imgUrl}) top center no-repeat` }}
  >
    {children}
    <button>
      <CaretIcon className="CaretIcon" />
      <span>Read more</span>
    </button>
    <GradientRibbon />
  </Styled>
);

export default Card;
