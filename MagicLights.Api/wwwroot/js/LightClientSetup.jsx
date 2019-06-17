class LightClientSetup extends React.Component {
  constructor(props) {
    super(props);

    this.onConfigurationChanged = this.onConfigurationChanged.bind(this);
    this.onEnableChanged = this.onEnableChanged.bind(this);
    this.setClientValue = this.setClientValue.bind(this);
    this.state = {
      value: props.value
    };
  }

  setClientValue(id, field, value) {
    this.setState(prevState => {
      var client = prevState.value.find((c) => c.id === id);
      var clientIndex = prevState.value.indexOf(client);

      var newClient = {...client};
      newClient.value = {...newClient.value};
      newClient.value[field] = value;

      var newValue = prevState.value.slice();
      newValue[clientIndex] = newClient;

      var newState = {...prevState};
      newState.value = newValue;

      this.props.onChange(newState.value);

      return newState;
    });
  }

  onEnableChanged(id, value) {
    this.setClientValue(id, "enabled", value);
  }

  onConfigurationChanged(id, value) {
    this.setClientValue(id, "config", value);
  }

  render() {
    let clients = this.state.value.map(client => {
      let value = client.value;
      return (
        <LightClient 
          key={client.id}
          id={client.id}
          enabled={value.enabled}
          config={value.config || {}} 
          settings={client.settings || []}
          onEnableChanged={this.onEnableChanged}
          onConfigurationChanged={this.onConfigurationChanged}
        />)
    });
    return (
      <div>
        {clients}
      </div>
    );
  }
}
