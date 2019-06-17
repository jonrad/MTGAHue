class Configuration extends React.Component {
  constructor(props) {
    super(props);

    let values = {...props.value} || props.settings.map(s => {
      return {[s.id]: s.defaultValue};
    })

    this.state = {
      value: values,
      settings: props.settings
    };

    this.onSettingChange = this.onSettingChange.bind(this);
  }

  onSettingChange(setting, value) {
    this.setState(state => {
      state.value[setting] = value;

      this.props.onChanged && this.props.onChanged(state.value);
      return state;
    });
  }

  render() {
    let inputs = this.state.settings.map((c) => {
      const value = this.state.value[c.id];
      return (
        <div key={c.id} className="form-group row">
          <label className="col-sm-2 col-form-label">{c.id}</label>
          <div className="col-sm-4">
            <Setting 
              id={c.id}
              settings={c} 
              value={value}
              onChange={this.onSettingChange} />
          </div>
        </div>
      );
    });

    return (
      <div>
        {inputs}
      </div>
    );
  }
}