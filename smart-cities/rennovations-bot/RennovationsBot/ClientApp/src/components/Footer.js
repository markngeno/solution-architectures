import React from "react";
import styled from "styled-components";
import FooterLogo from "../icons/city-seal_medium";
import FacebookIcon from "../icons/facebook_icon";
import TwitterIcon from "../icons/twitter_icon";
import LinkedInIcon from "../icons/linked-in_icon";

const Styled = styled.footer`
  &.Footer {
    position: relative;

    .logo-container {
      background-color: ${props => props.theme.color_background};
      position: absolute;
      top: 0;
      left: 50%;
      padding: 15px;
      border-radius: 50%;
      transform: translate(-50%, -50%);

      .FooterLogo {
        width: 100px;
        height: 100px;
      }
    }
    .content {
      padding-top: 95px;
      text-align: center;

      b {
        display: block;
        font-weight: normal;
        font-size: 2.25rem;
        color: ${props => props.theme.color_link};
        margin-bottom: 20px;
      }
      span {
        display: block;
        font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
          sans-serif;
        font-size: 1.125;
        font-weight: normal;
        color: #8d8b8b;
        line-height: 1.3;
      }
      li {
        display: inline-block;
      }
      .social-media {
        > li {
          margin: 25px 15px 35px 0;

          &:last-child {
            margin-right: 0;
          }

          > button {
            svg {
              width: 2rem;
              height: 2rem;
            }
          }
        }
      }
      .footer-links {
        background: ${props => props.theme.blue_gradient};

        > li {
          margin: 15px 20px 17px;

          > button {
            font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
              sans-serif;
            font-size: 1.125;
            color: white;
          }
        }
      }
    }
  }
`;

const Footer = () => (
  <Styled className="Footer">
    <div className="logo-container">
      <FooterLogo className="FooterLogo" />
    </div>
    <div className="content">
      <b>City and County of Contoso</b>
      <span>Phone: 202-555-0155</span>
      <span>Fax: 202-555-0119</span>
      <ul className="social-media">
        <li>
          <button>
            <FacebookIcon />
          </button>
        </li>
        <li>
          <button>
            <TwitterIcon />
          </button>
        </li>
        <li>
          <button>
            <LinkedInIcon />
          </button>
        </li>
      </ul>
      <ul className="footer-links">
        <li>
          <button>ADA notice</button>
        </li>
        <li>
          <button>Notice of Nondiscrimination</button>
        </li>
        <li>
          <button>Privacy</button>
        </li>
        <li>
          <button>&copy; Copyright 2018 City of Contoso</button>
        </li>
      </ul>
    </div>
  </Styled>
);

export default Footer;
