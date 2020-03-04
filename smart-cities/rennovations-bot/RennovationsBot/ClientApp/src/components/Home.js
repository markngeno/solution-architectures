import React, { Component } from "react";
import styled from "styled-components";
import CitySeal from "../icons/city-seal_large";
import BannerImg from "../images/BannerImage.jpg";
import Tile1Img from "../images/Tile1_Image.png";
import Tile2Img from "../images/Tile2_Image.png";
import Tile3Img from "../images/Tile3_Image.png";
import Tile4Img from "../images/Tile4_Image.png";
import Tile5Img from "../images/Tile5_Image.png";
import Tile6Img from "../images/Tile6_Image.png";
import Card from "./Card";
import GradientRibbon from "./GradientRibbon";

const Styled = styled.div`
  &.Home {
    section {
      .container {
        margin: 0 auto;
        width: 1020px;

        .card-container {
          display: flex;
          justify-content: space-between;

          .Card {
            width: 262px;
          }
        }
      }

      &.top-section {
        box-shadow: 0px 5px 8px 1px rgba(0, 0, 0, 0.18);
        background: ${props => props.theme.color_background} url(${BannerImg})
          center top no-repeat;
        padding-top: 20px;
        padding-bottom: 45px;

        .CitySeal {
          width: 200px;
          height: 200px;
          margin: 0 auto;
          display: block;
        }

        .intro {
          position: relative;
          background-color: white;
          margin: 25px 0 40px;
          padding: 30px 30px 35px;
          font-family: SegoeUISemilight, "Helvetica Neue", Helvetica, Arial,
            sans-serif;
          font-size: 1.5rem;
          color: ${props => props.theme.color_text};
          box-shadow: ${props => props.theme.box_shadow};

          h1 {
            font-size: 2.25rem;
            font-weight: normal;
            margin: 0 0 20px;
          }
          p {
            line-height: 1.3;
          }
        }
      }

      &.link-section {
        h2 {
          font-family: SegoeUISemilight, "Helvetica Neue", Helvetica, Arial,
            sans-serif;
          font-size: 1.5rem;
          font-weight: normal;
          margin: 40px 0 24px;
          color: ${props => props.theme.color_text};
        }
        ul {
          display: flex;
          flex-wrap: wrap;
          justify-content: space-between;
          padding-bottom: 20px;

          li {
            display: inline-block;
            margin-bottom: 20px;

            button {
              font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
                sans-serif;
              width: 173px;
              height: 40px;
              border: solid 1px ${props => props.theme.color_link};
              background-color: ${props => props.theme.color_link};
              color: white;
              text-align: center;
              transition: all 0.3s;

              :hover {
                background-color: white;
                color: ${props => props.theme.color_link};
              }
            }
          }
        }
      }
      &.bottom-section {
        background-color: ${props => props.theme.color_background};
        padding-top: 40px;
        padding-bottom: 90px;
      }
    }
  }
`;

class Home extends Component {
  render() {
    return (
      <Styled className="Home">
        <section className="top-section">
          <div className="container">
            <CitySeal className="CitySeal" />
            <div className="intro">
              <h1>Contoso City Council</h1>
              <p>
                Residents of the city and county of Contoso, the city council is
                here to serve you. Together we write the laws that govern
                Contoso and build the districts and communities that we’re all
                so proud of. Contoso is a unique city that offers residents
                access to many opportunities and services. Explore the site or
                engage our Smart City bot to find out more.
              </p>
              <GradientRibbon />
            </div>
            <div className="card-container">
              <Card imgUrl={Tile1Img}>
                <b>The Environment</b>
                <p>
                  The council is taking definitive steps towards tackling
                  climate change with the 2022 carbon emmissions plan. Click
                  here to read more about the initiatives underway and how you
                  can help.
                </p>
              </Card>
              <Card imgUrl={Tile2Img}>
                <b>Health &amp; Social Services</b>
                <p>
                  Access information about the City and County of Contoso’s
                  disability programs and community based agencies. Find out
                  more about the assitance packages available for those in need.
                </p>
              </Card>
              <Card imgUrl={Tile3Img}>
                <b>The Environment</b>
                <p>
                  Learn about the major legal cases that the City Attorney's
                  Office is pursuing. The Major Cases section offers access to
                  news about by the Office's work on matters of particular
                  public interest.
                </p>
              </Card>
            </div>
          </div>
        </section>
        <section className="link-section">
          <div className="container">
            <h2>Quick links for residents</h2>
            <ul>
              <li>
                <button>Property Search</button>
              </li>
              <li>
                <button>Parking</button>
              </li>
              <li>
                <button>Rubbish &amp; Recycling</button>
              </li>
              <li>
                <button>Building Consents</button>
              </li>
              <li>
                <button>District Plan</button>
              </li>
              <li>
                <button>Projects</button>
              </li>
              <li>
                <button>Hearings</button>
              </li>
              <li>
                <button>Meet the Council</button>
              </li>
              <li>
                <button>News</button>
              </li>
              <li>
                <button>Committees</button>
              </li>
            </ul>
          </div>
        </section>
        <section className="bottom-section">
          <div className="container">
            <div className="card-container">
              <Card imgUrl={Tile4Img}>
                <b>Committees</b>
                <p>
                  Committees are formed to undertake special projects around the
                  city and to debate key topics to make sure we achieve the best
                  balance for everyone. Find out what’s going on.
                </p>
              </Card>
              <Card imgUrl={Tile5Img}>
                <b>News</b>
                <p>
                  This is the single source of truth for what’s happening around
                  the city, what decisions are being made in council, personnel
                  changes, road closures and other information relevant for
                  residents.
                </p>
              </Card>
              <Card imgUrl={Tile6Img}>
                <b>Projects</b>
                <p>
                  Currently there are several major transportation and
                  environmental impact projects underway in the city. Keep
                  across the progress, and learn about any delays that might
                  impact you.
                </p>
              </Card>
            </div>
          </div>
        </section>
      </Styled>
    );
  }
}

export default Home;
