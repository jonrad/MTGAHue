import React from 'react';
import Setting from './Setting';

class Effect extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      value: props.value,
      settings: props.settings
    };

    this.onSettingChange = this.onSettingChange.bind(this);
  }

  onSettingChange(setting, value) {
    this.setState({
      value: {
        config: {
          [setting]: value
        }
      }
    });

    this.props.onSettingChange && this.props.onSettingChange(setting, value);
  }

  render() {
    const value = this.state.value;
    const configValues = value.config;

    let inputs = this.state.settings.configuration.map((c) => 
    {
      const value = configValues ? configValues[c.id] : c.default;
      return (
        <div key={c.id} className="form-group row">
          <label className="col-sm-2 col-form-label">{c.id}</label>
          <div className="col-sm-4">
            <Setting settings={c} value={value} onChange={(value) => this.onSettingChange(c.id, value)} />
          </div>
        </div>
      );
    });
    return (
      <div>
        <div className="form-group row">
          {this.state.settings.id}
        </div>
        {inputs}
      </div>
    );
  }
}

export default Effect;
