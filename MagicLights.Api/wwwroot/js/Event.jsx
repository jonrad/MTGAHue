import React from 'react';
import Effect from './Effect';

class Event extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      value: props.value
    };

    this.effectChange = this.effectChange.bind(this);
    this.onSettingChange = this.onSettingChange.bind(this);
  }

  effectChange(e) {
    this.setState({
      value: {
        effect: {
          id: e.target.value
        }
      }
    });
  }

  onSettingChange(setting, value) {
    this.setState((state) => {
      return {
        value: {
          effect: {
            id: state.value.effect.id,
            config: {
              [setting]: value
            }
          }
        }
      };
    });
  }

  render() {
    let options = this.props.settings.effects.map(e => {
      return (
        <option key={e.id} value={e.id}>{e.id}</option>
      );
    });

    let effectValue = this.state.value.effect;
    let effect = this.props.settings.effects.find(e => e.id === effectValue.id);

    return (
      <div>
        <div>
          Event: {this.props.settings.id}
        </div>
        <div>
          <select value={effectValue && effectValue.id} onChange={this.effectChange}>
            <option value="">None</option>
            {options}
          </select>
          {effect &&
            <div>
              <Effect 
                key={effect.id}
                value={this.state.value.effect} 
                settings={effect}
                onSettingChange={this.onSettingChange}
              />
            </div>
          }
        </div>
      </div>
    );
  }
}

export default Event;