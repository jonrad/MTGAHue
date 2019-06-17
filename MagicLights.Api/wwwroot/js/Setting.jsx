class Setting extends React.Component {
  constructor(props) {
    super(props);
    this.state = { value: props.value };
    this.handleChange = this.handleChange.bind(this);
    this.handleBlur = this.handleBlur.bind(this);
  }

  handleChange(e) {
    this.setState({ value: e.target.value });
  }

  handleBlur() {
    this.props.onChange(this.props.id, this.state.value);
  }

  render() {
    return (
      <input
        className="form-control"
        value={this.state.value || ""}
        onChange={this.handleChange}
        onBlur={this.handleBlur} />
    );
  }
}
