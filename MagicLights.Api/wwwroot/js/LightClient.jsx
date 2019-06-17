class LightClient extends React.Component {
  constructor(props) {
    super(props);

    this.enableChanged = this.enableChanged.bind(this);
    this.configurationChanged = this.configurationChanged.bind(this);
  }

  enableChanged(e) {
    this.props.onEnableChanged(
      this.props.id,
      !this.props.enabled
    );
  }

  configurationChanged(config) {
    this.props.onConfigurationChanged(
      this.props.id,
      config);
  }

  render() {
    let checkBoxControlId = this.props.id + "EnabledCheckbox";
    return (
      <div className="card">
        <div className="card-header form-group form-check">
          <h5>
            <input
              type="checkbox"
              className="form-control-input"
              id={checkBoxControlId}
              checked={this.props.enabled}
              onChange={this.enableChanged} />
            <label className="form-check-label" htmlFor={checkBoxControlId}>
              &nbsp;{this.props.id}
            </label>
          </h5>
        </div>
        <div className="card-body">
          More configuration to come...
          <Configuration 
            value={this.props.config} 
            settings={this.props.settings}
            onChanged={this.configurationChanged}
          />
        </div>
      </div>
    );
  };
}