import React, { Component } from "react";
import { connect } from "react-redux";
import styled from "styled-components";
import BulletIcon from "../icons/bullet_icon";
import CaretIcon from "../icons/caret_icon";
import DownloadIcon from "../icons/download_icon";
import PrintIcon from "../icons/print_icon";
import ShareIcon from "../icons/share_icon";
import GradientRibbon from "./GradientRibbon";
import ScrollToTopOnMount from "./ScrollToTopOnMount";
import { PERMIT_FORM } from "../common/constants";
import * as actionCreators from "../redux/actions";

const Styled = styled.div`
  &.PermitForm {
    padding: 30px 0 90px;
    background-color: ${props => props.theme.color_background};

    .container {
      display: grid;
      grid-gap: 30px;
      grid-template-rows: auto auto;
      grid-template-columns: 1fr 2fr;
      margin: 0 auto;
      width: 1020px;
      background-color: white;
      padding: 0 30px;
      box-sizing: border-box;
      box-shadow: ${props => props.theme.box_shadow};

      header {
        grid-row: 1 / 2;
        grid-column: 1 / span 2;
        position: relative;

        h1 {
          font-family: SegoeUISemilight, "Helvetica Neue", Helvetica, Arial,
            sans-serif;
          font-weight: normal;
          margin: 20px 0 15px;
        }

        .icons {
          position: absolute;
          top: 28px;
          right: 0;

          button {
            margin-left: 1rem;
          }

          svg {
            width: 1.3rem;
            height: 1.2rem;
          }
        }
      }
      article {
        em {
          display: block;
          font-family: SegoeUISemilight, "Helvetica Neue", Helvetica, Arial,
            sans-serif;
          font-size: 1.5rem;
          margin-bottom: 20px;
          font-style: normal;
        }
        p {
          font-size: 0.875rem;
          line-height: 1.3;
          margin: 0 0 30px;
        }
        form {
          background-color: ${props => props.theme.color_form_background};
          margin-bottom: 30px;

          &.is-invalid {
            .std-text-field input {
              outline: 1px solid red;
            }
          }

          fieldset {
            margin: 0;
            padding: 0 20px;
            border: 0 none;

            &:last-of-type {
              legend {
                padding-top: 2px;
              }
            }

            > div,
            > label {
              margin-bottom: 12px;
            }

            > div {
              clear: both;

              > span {
                display: block;
                font-size: 0.75rem;
                color: #8a8d90;
                margin-bottom: 3px;
              }
            }
          }

          legend {
            font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
              sans-serif;
            display: block;
            color: ${props => props.theme.color_tertiary};
            margin: 0;
            padding: 20px 0 7px;
          }

          label {
            display: block;
            > span {
              display: block;
              font-size: 0.75rem;
              color: #8a8d90;
              margin-bottom: 4px;
            }
            input[type="text"] {
              padding: 7px 10px;
              width: 100%;
              border: 0 none;
            }
            input[type="radio"] {
              position: absolute !important;
              clip: rect(1px 1px 1px 1px); /* IE6, IE7 */
              clip: rect(1px, 1px, 1px, 1px);
              padding: 0 !important;
              border: 0 !important;
              height: 1px !important;
              width: 1px !important;
              overflow: hidden;

              &:checked + .icon-bullet .icon-foreground.inner {
                fill: #003da5;
              }

              + .icon-bullet {
                display: inline-block;
                margin-right: 10px;
                height: 0.9rem;
                width: 0.9rem;
                vertical-align: -2px;

                .inner {
                  transition: fill 0.3s;
                }

                + span {
                  display: inline-block;
                  font-size: 0.875rem;
                  color: #000000;
                }
              }
            }
          }

          textarea {
            padding: 8px 10px;
            resize: none;
            width: 90%;
            border: 0 none;
            font-size: 0.875rem;
          }

          button[type="submit"] {
            font-family: SegoeUISemibold, "Helvetica Neue", Helvetica, Arial,
              sans-serif;
            background-color: ${props => props.theme.color_link};
            border: 1px solid ${props => props.theme.color_link};
            margin: 3px 20px 15px;
            padding: 10px 40px 12px;
            font-size: 1.125rem;
            color: white;
            transition: all 0.3s;

            &:hover {
              background-color: white;
              color: ${props => props.theme.color_link};
            }
          }

          .text-fields {
            overflow: hidden;

            label:first-child {
              width: 60%;
              float: left;

              input[type="text"] {
                width: 90%;
              }
            }
            label:last-child {
              width: 30%;
              float: left;
            }
          }
          .radio-fields {
            label {
              display: inline-block;
              margin-right: 20px;
            }
          }
          .std-text-field {
            max-width: 200px;
          }
        }
      }
      aside {
        grid-row: 2;
        grid-column: 1;

        h2 {
          margin: 0;
          padding: 1rem 24px;
          font-family: SegoeUISemilight, "Helvetica Neue", Helvetica, Arial,
            sans-serif;
          font-weight: normal;
          background-color: ${props => props.theme.color_form_background};
        }

        > ul > li {
          transition: max-height 0.4s;
          max-height: 46px;
          overflow: hidden;

          > button {
            display: block;
            border-top: 1px solid #cccccc;
            background-color: ${props => props.theme.color_form_background};
            padding: 13px 10px 15px 24px;
            width: 100%;
            text-align: left;
          }

          &.active {
            max-height: 200px;

            > button {
              background: transparent ${props => props.theme.blue_gradient}
                center left no-repeat;
              color: white;

              .CaretIcon {
                transform: rotate(90deg);

                path {
                  fill: white;
                }
              }
            }
          }

          > ul > li {
            margin: 14px 5px 16px 40px;
          }

          .CaretIcon {
            width: 9px;
            height: 15px;
            vertical-align: -2px;
            margin-right: 10px;

            path {
              fill: #000000;
            }
          }
        }
      }
    }
  }
`;

class PermitForm extends Component {
  constructor(props) {
    super(props);
    this.requirementsEl = React.createRef();
    this.state = {
      workValue: "",
      // 20000 equals 20 seconds
      inactivityTimer: setTimeout(this.inactivityTimerCallback, 20000)
    };
  }

  componentDidMount() {
    setTimeout(() => {
      this.requirementsEl.current.classList.add("active");
    }, 800);
  }

  componentDidUpdate(prevProps, prevState) {
    const { workValue } = this.state;
    if (workValue && workValue !== prevState.workValue) {
      const isValid = this.validateForm();
      this.setState({ invalidForm: !isValid });
    }
  }

  submitForm = event => {
    const { inactivityTimer } = this.state;
    event.preventDefault();
    clearInterval(inactivityTimer);
    const isValid = this.validateForm();
    if (isValid) {
      this.props.submitForm();
    } else {
      this.setState({ invalidForm: true, inactivityTimer: setTimeout(this.inactivityTimerCallback, 20000) });
      this.props.correctForm([6]);
    }
  };

  inactivityTimerCallback = () => {
    const { inactivityTimer } = this.state;
    clearTimeout(inactivityTimer);
    this.props.inactivityForm();
  }

  trackFormChange = event => {
    const { inactivityTimer } = this.state;
    clearTimeout(inactivityTimer);
    this.setState({
      inactivityTimer: setTimeout(this.inactivityTimerCallback, 20000)
    });
  };

  updateRadio = event => {
    const name = event.currentTarget.name;
    const value = event.currentTarget.value;
    this.setState({ [name]: value }, () => {
      const stateKeys = Object.keys(this.state);
      const keysToFind = [
        "propertyType",
        "permitType",
        "permitPurpose",
        "permitPlumbing",
        "permitElectrical",
        "permitMechanical"
      ];
      if (keysToFind.every(key => stateKeys.includes(key))) {
        const isValid = this.validateForm();
        if (!isValid) {
          this.setState({ invalidForm: true });
          this.props.correctForm([6]);
        }
      }
    });
  };

  updateText = event => {
    const name = event.currentTarget.name;
    const value = event.currentTarget.value;
    this.setState({ [name]: value });
  };

  validateForm = () => {
    // TODO: Validate all fields.
    // The priority is to validate the field in question number 6 for the purposes of the demo
    let isValid = true;
    if (this.state.workValue === "") {
      isValid = false;
    }
    return isValid;
  };

  render() {
    const { user } = this.props;
    const {
      invalidForm,
      propertyType,
      permitType,
      permitPurpose,
      permitPlumbing,
      permitElectrical,
      permitMechanical,
      workValue
    } = this.state;
    const validClass = invalidForm ? "is-invalid" : "is-valid";
    return (
      <Styled className="PermitForm">
        <ScrollToTopOnMount />
        <div className="container">
          <header>
            <h1>Check if you need a permit</h1>
            <div className="icons">
              <button>
                <PrintIcon />
              </button>
              <button>
                <ShareIcon />
              </button>
              <button>
                <DownloadIcon />
              </button>
            </div>
            <GradientRibbon />
          </header>
          <article>
            <em>
              A Building Permit is required prior to constructing, enlarging,
              altering, repairing or changing the use of a building or
              structure. Use the Building Permit Checklist to ensure that all
              required documents are provided.
            </em>
            <p>
              All building work done in the city must comply with the Building
              Code, and many projects will need a comprehensive building permit.
              If you are undertaking 'low-risk' building work, such as laying a
              patio or installing kitchen cupboards, it may be eligible for a
              expidited permit under the Building Code. Fill out the form below
              to determine the permit type required for your project.
            </p>
            <form onSubmit={this.submitForm} className={validClass} onChange={this.trackFormChange}>
              <fieldset>
                <legend>Property Information</legend>
                <div className="text-fields">
                  <label>
                    <span>1. Site Address:</span>
                    <input
                      type="text"
                      defaultValue="254, 15th Avenue East, Northwind Hills"
                    />
                  </label>
                  <label>
                    <span>2. Section/Lot ID:</span>
                    <input type="text" defaultValue="D-38A771-442" />
                  </label>
                </div>
                <div className="text-fields">
                  <label>
                    <span>3. Property Owner:</span>
                    <input type="text" defaultValue={user.name} />
                  </label>
                  <label>
                    <span>4. Owner Phone:</span>
                    <input type="text" defaultValue="202-555-0155" />
                  </label>
                </div>
                <div className="radio-fields">
                  <span>
                    5. The property is currently: <i>(Select one)</i>
                  </span>
                  <label>
                    <input
                      type="radio"
                      name="propertyType"
                      value={PERMIT_FORM.PropertyType.OWNER}
                      checked={
                        propertyType === `${PERMIT_FORM.PropertyType.OWNER}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>Owner occupied</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="propertyType"
                      value={PERMIT_FORM.PropertyType.TENANTED}
                      checked={
                        propertyType === `${PERMIT_FORM.PropertyType.TENANTED}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>Tenanted (commercial permits may apply)</span>
                  </label>
                </div>
              </fieldset>
              <fieldset>
                <legend>Permit Information</legend>
                <label className="std-text-field">
                  <span>6. The fair market value of the work:</span>
                  <input
                    type="text"
                    name="workValue"
                    value={workValue}
                    onChange={this.updateText}
                  />
                </label>
                <div>
                  <span>
                    7. The permit involves: <i>(Select one)</i>
                  </span>
                  <label>
                    <input
                      type="radio"
                      name="permitType"
                      value={PERMIT_FORM.PermitType.SINGLE_FAMILY}
                      checked={
                        permitType === `${PERMIT_FORM.PermitType.SINGLE_FAMILY}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>
                      Single Family, Duplex or Townhouse (Residential Only)
                    </span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitType"
                      value={PERMIT_FORM.PermitType.MULTI_FAMILY}
                      checked={
                        permitType === `${PERMIT_FORM.PermitType.MULTI_FAMILY}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>Multi-Family (Residential Only)</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitType"
                      value={PERMIT_FORM.PermitType.MIXED_USE}
                      checked={
                        permitType === `${PERMIT_FORM.PermitType.MIXED_USE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>
                      Mixed-Use (Residential plus commercial or industrial)
                    </span>
                  </label>
                </div>
                <div>
                  <span>
                    8. The permit is for: <i>(Select one)</i>
                  </span>
                  <label>
                    <input
                      type="radio"
                      name="permitPurpose"
                      value={PERMIT_FORM.PermitPurpose.CONSTRUCTION}
                      checked={
                        permitPurpose ===
                        `${PERMIT_FORM.PermitPurpose.CONSTRUCTION}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>Construction of a NEW building</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitPurpose"
                      value={PERMIT_FORM.PermitPurpose.ADDITION}
                      checked={
                        permitPurpose ===
                        `${PERMIT_FORM.PermitPurpose.ADDITION}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>Addition to an EXISTING building</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitPurpose"
                      value={PERMIT_FORM.PermitPurpose.ALTERATION}
                      checked={
                        permitPurpose ===
                        `${PERMIT_FORM.PermitPurpose.ALTERATION}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>Alteration of an EXISTING building</span>
                  </label>
                </div>
                <div>
                  <span>
                    9. Do you intend to include plumbing work with this building
                    permit? <i>(Select one)</i>
                  </span>
                  <label>
                    <input
                      type="radio"
                      name="permitPlumbing"
                      value={PERMIT_FORM.PermitPlumbing.NONE}
                      checked={
                        permitPlumbing === `${PERMIT_FORM.PermitPlumbing.NONE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>No, there is no plumbing work</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitPlumbing"
                      value={PERMIT_FORM.PermitPlumbing.SEPARATE}
                      checked={
                        permitPlumbing ===
                        `${PERMIT_FORM.PermitPlumbing.SEPARATE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>No, a separate Plumbing Permit will be obtained</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitPlumbing"
                      value={PERMIT_FORM.PermitPlumbing.SUPPLEMENTAL}
                      checked={
                        permitPlumbing ===
                        `${PERMIT_FORM.PermitPlumbing.SUPPLEMENTAL}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>
                      Yes, supplemental plumbing information will be provided
                      upon request
                    </span>
                  </label>
                </div>
                <div>
                  <span>
                    10. Is electrical work taking place as part of this project?
                    <i>(Select one)</i>
                  </span>
                  <label>
                    <input
                      type="radio"
                      name="permitElectrical"
                      value={PERMIT_FORM.PermitElectrical.NONE}
                      checked={
                        permitElectrical ===
                        `${PERMIT_FORM.PermitElectrical.NONE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>No, there is no electrical work</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitElectrical"
                      value={PERMIT_FORM.PermitElectrical.SEPARATE}
                      checked={
                        permitElectrical ===
                        `${PERMIT_FORM.PermitElectrical.SEPARATE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>
                      Yes, a separate Electrical Permit will be obtained
                    </span>
                  </label>
                </div>
                <div>
                  <span>
                    11. Do you intend to include mechanical work with this
                    building permit? <i>(Select one)</i>
                  </span>
                  <label>
                    <input
                      type="radio"
                      name="permitMechanical"
                      value={PERMIT_FORM.PermitMechanical.NONE}
                      checked={
                        permitMechanical ===
                        `${PERMIT_FORM.PermitMechanical.NONE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>No, there is no mechanical work</span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitMechanical"
                      value={PERMIT_FORM.PermitMechanical.SEPARATE}
                      checked={
                        permitMechanical ===
                        `${PERMIT_FORM.PermitMechanical.SEPARATE}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>
                      No, a separate Mechanical Permit will be obtained
                    </span>
                  </label>
                  <label>
                    <input
                      type="radio"
                      name="permitMechanical"
                      value={PERMIT_FORM.PermitMechanical.SUPPLEMENTAL}
                      checked={
                        permitMechanical ===
                        `${PERMIT_FORM.PermitMechanical.SUPPLEMENTAL}`
                      }
                      onChange={this.updateRadio}
                    />
                    <BulletIcon />
                    <span>
                      Yes, supplemental mechanical information will be provided
                      upon request
                    </span>
                  </label>
                </div>
                <label>
                  <span>
                    12. Description of the work: (plus any additional notes)
                  </span>
                  <textarea rows="3" defaultValue="Kitchen remodelling" />
                </label>
              </fieldset>
              <button type="submit">Submit</button>
            </form>
          </article>
          <aside>
            <h2>Permit Requirements</h2>
            <ul>
              <li ref={this.requirementsEl}>
                <button>
                  <CaretIcon className="CaretIcon" />
                  <span>Check if you need a permit</span>
                </button>
                <ul>
                  <li>
                    <CaretIcon className="CaretIcon" />
                    Resource consent
                  </li>
                  <li>
                    <CaretIcon className="CaretIcon" />
                    Building consent exemptions
                  </li>
                  <li>
                    <CaretIcon className="CaretIcon" />
                    Plumbing and drainage work
                  </li>
                  <li>
                    <CaretIcon className="CaretIcon" />
                    Engineer-designed exemptions
                  </li>
                </ul>
              </li>
              <li>
                <button>
                  <CaretIcon className="CaretIcon" />
                  <span>Authorized contractors</span>
                </button>
              </li>
              <li>
                <button>
                  <CaretIcon className="CaretIcon" />
                  <span>Building information modelling</span>
                </button>
              </li>
            </ul>
          </aside>
        </div>
      </Styled>
    );
  }
}

export default connect(
  null,
  actionCreators
)(PermitForm);
